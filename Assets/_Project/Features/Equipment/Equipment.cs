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
    public Vector3 VisualPrefabEulerOffset = Vector3.zero;

    public abstract void InitializeGameplay(
        MechController mech,
        EquipmentSlot slot,
        bool isPlayer,
        InputActionReference inputActionRef);

    protected T initializeRuntimeComponent<T>(MechController mech, EquipmentSlot slot) where T : MechEquipmentRuntime
    {
        var _newObj = new GameObject($"{slot.SlotType}_{DisplayName}");
        _newObj.transform.SetParent(slot.TransformComponent);
        _newObj.transform.localPosition = Vector3.zero;
        _newObj.transform.localRotation = Quaternion.identity;

        var _runtimeComponent = _newObj.AddComponent<T>();
        _runtimeComponent.InitializeGameplay(mech, this);

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
}
