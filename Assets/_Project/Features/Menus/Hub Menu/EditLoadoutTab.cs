using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditLoadoutTab : UITab
{
    [Header("Object References")]
    [SerializeField] private EquipmentUIElementDictionary m_equipmentElementDictionary = new();
    [Space]
    [SerializeField] private MechLoadout m_loadoutAsset = null;
    [SerializeField] private DataPanel m_equipmentDataPanel = null;
    [SerializeField] private EquipmentSlotEditTab m_slotEditTab = null;

    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new();

    protected override void onOpened()
    {
        base.onOpened();

        m_equipmentDataPanel.gameObject.SetActiveOptimized(false);

        rebuild();
    }

    protected override void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        base.onActiveInputDeviceChanged(deviceType);

        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        if (getActiveInputDevice() != InputDeviceTypes.KeyboardAndMouse)
            EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, m_equipmentElementDictionary[EquipmentSlotTypes.LeftArm].gameObject);
    }

    protected override void onCancelInput(InputAction.CallbackContext context)
    {
        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        DevelopmentScreen.Instance.OpenTab(0);
    }

    private void rebuild()
    {
        m_loadoutList.AllLoadouts.Clear();
        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);
        m_loadoutAsset.PopulateFromSerializedData(m_loadoutList.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex]);

        foreach (var _kvp in m_loadoutAsset.Dictionary)
        {
            var _slotType = _kvp.Key;
            var _equipmentAsset = _kvp.Value;

            var _uiElement = m_equipmentElementDictionary[_slotType];

            _uiElement.Initialize(
                equipment: _equipmentAsset,
                onSelectedCallback: () => onEquipmentSelected(_equipmentAsset),
                onClickedCallback: () => onSlotClicked(_slotType));
        }

        if (getActiveInputDevice() != InputDeviceTypes.KeyboardAndMouse)
            EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, m_equipmentElementDictionary[EquipmentSlotTypes.LeftArm].gameObject);
    }

    private void onEquipmentSelected(Equipment equipment)
    {
        m_equipmentDataPanel.Clear();

        if (equipment == null)
            m_equipmentDataPanel.PopulateWithEmptyData();
        else
            equipment.PopulateDataPanel(m_equipmentDataPanel);

        m_equipmentDataPanel.gameObject.SetActiveOptimized(true);
    }

    private void onSlotClicked(EquipmentSlotTypes slotType)
    {
        m_slotEditTab.SetSlot(slotType);
        DevelopmentScreen.Instance.OpenTab(2);
    }

    [ContextMenu("Clear UI Element Dictionary")]
    private void clearUIElementDictionary()
    {
        m_equipmentElementDictionary.Clear();

        foreach (int i in Enum.GetValues(typeof(EquipmentSlotTypes)))
        {
            if (i < 0)
                continue;

            m_equipmentElementDictionary.Add((EquipmentSlotTypes)i, null);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
    }
}