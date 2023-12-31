using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public abstract class Equipment : ScriptableObject
{
    [Header("Equipment Generic Settings")]
    public GUIDWrapper GUID = new GUIDWrapper();
    public string DisplayName = "New Equipment";
    [TextArea] public string Description = "Description";
    public EquipmentCategory Category = null;
    [Space]
    [Min(0)] public int Weight = 100;
    [Min(0)] public int EnergyDrain = 100;
    [Space]
    public GameObject VisualsPrefab = null;
    public Vector3 VisualPrefabPositionOffset = Vector3.zero;
    public Vector3 VisualPrefabEulerOffset = Vector3.zero;

    public struct EquipmentRuntimeSetupData
    {
        public MechController Mech;
        public Transform SlotTransform;
        public EquipmentSlotTypes SlotType;
        public bool IsPlayer;
        public InputActionReference InputActionRef;
    }

    public abstract void InitializeGameplay(EquipmentRuntimeSetupData setupData);

    protected T initializeRuntimeComponent<T>(EquipmentRuntimeSetupData setupData) where T : MechEquipmentRuntime
    {
        var _runtimeComponent = setupData.SlotTransform.gameObject.AddComponent<T>();
        _runtimeComponent.InitializeGameplay(setupData.Mech, this, setupData.SlotType);
        return _runtimeComponent;
    }

    public void PopulateDataPanel(DataPanel dataPanel)
    {
        dataPanel.CreateTextElement().Initialize(DisplayName, Category.DisplayName, 24, 18);
        dataPanel.CreateDivider().SetPreferredHeight(14);
        dataPanel.CreateTextElement().Initialize(Description, 18);
        dataPanel.CreateDivider().SetPreferredHeight(14);

        populateDataPanel_Custom(dataPanel);

        dataPanel.CreateDivider().SetPreferredHeight(14);
        dataPanel.CreateTextElement().Initialize("Weight", Weight.ToStringMinimalAlloc(),  18, 18);
        dataPanel.CreateTextElement().Initialize("Energy Drain", EnergyDrain.ToStringMinimalAlloc(), 18, 18);
    }

    protected virtual void populateDataPanel_Custom(DataPanel dataPanel) { }

    [ContextMenu("Regenerate GUID")]
    public void Editor_RegenerateGUID()
    {
#if UNITY_EDITOR
        GUID = new GUIDWrapper();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
    }
}
