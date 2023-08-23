using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditLoadoutTab : UITab
{
    [Header("Object References")]
    [SerializeField] private EquipmentUIElement m_lArm = null;
    [SerializeField] private EquipmentUIElement m_rArm = null;
    [SerializeField] private EquipmentUIElement m_lShoulder = null;
    [SerializeField] private EquipmentUIElement m_rShoulder = null;
    [SerializeField] private EquipmentUIElement m_melee = null;
    [SerializeField] private EquipmentUIElement m_generator = null;
    [SerializeField] private EquipmentUIElement m_head = null;
    [SerializeField] private EquipmentUIElement m_body = null;
    [SerializeField] private EquipmentUIElement m_arms = null;
    [SerializeField] private EquipmentUIElement m_legs = null;
    [SerializeField] private EquipmentUIElement m_utility1 = null;
    [SerializeField] private EquipmentUIElement m_utility2 = null;
    [SerializeField] private EquipmentUIElement m_utility3 = null;
    [SerializeField] private EquipmentUIElement m_utility4 = null;
    [SerializeField] private EquipmentUIElement m_passive1 = null;
    [SerializeField] private EquipmentUIElement m_passive2 = null;
    [SerializeField] private EquipmentUIElement m_passive3 = null;
    [SerializeField] private EquipmentUIElement m_passive4 = null;
    [Space]
    [SerializeField] private MechLoadout m_loadoutAsset = null;
    [SerializeField] private DataPanel m_equipmentDataPanel = null;

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

        if (IsOpened == false)
            return;

        if (deviceType != InputDeviceTypes.KeyboardAndMouse)
            EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, m_lArm.gameObject);
    }

    protected override void onCancelInput(InputAction.CallbackContext context)
    {
        if (IsOpened == false)
            return;

        DevelopmentScreen.Instance.OpenTab(0);
    }

    private void rebuild()
    {
        m_loadoutList.AllLoadouts.Clear();
        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);
        m_loadoutAsset.PopulateFromSerializedData(m_loadoutList.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex]);

        m_lArm.Initialize(m_loadoutAsset.LeftArm, () => onEquipmentSelected(m_loadoutAsset.LeftArm));
        m_rArm.Initialize(m_loadoutAsset.RightArm, () => onEquipmentSelected(m_loadoutAsset.RightArm));
        m_lShoulder.Initialize(m_loadoutAsset.LeftShoulder, () => onEquipmentSelected(m_loadoutAsset.LeftShoulder));
        m_rShoulder.Initialize(m_loadoutAsset.RightShoulder, () => onEquipmentSelected(m_loadoutAsset.RightShoulder));

        m_melee.Initialize(m_loadoutAsset.Melee, () => onEquipmentSelected(m_loadoutAsset.Melee));
        m_generator.Initialize(m_loadoutAsset.Generator, () => onEquipmentSelected(m_loadoutAsset.Generator));

        m_head.Initialize(m_loadoutAsset.Head, () => onEquipmentSelected(m_loadoutAsset.Head));
        m_arms.Initialize(m_loadoutAsset.Arms, () => onEquipmentSelected(m_loadoutAsset.Arms));
        m_body.Initialize(m_loadoutAsset.Body, () => onEquipmentSelected(m_loadoutAsset.Body));
        m_legs.Initialize(m_loadoutAsset.Legs, () => onEquipmentSelected(m_loadoutAsset.Head));

        m_utility1.Initialize(m_loadoutAsset.Utility1, () => onEquipmentSelected(m_loadoutAsset.Utility1));
        m_utility2.Initialize(m_loadoutAsset.Utility2, () => onEquipmentSelected(m_loadoutAsset.Utility2));
        m_utility3.Initialize(m_loadoutAsset.Utility3, () => onEquipmentSelected(m_loadoutAsset.Utility3));
        m_utility4.Initialize(m_loadoutAsset.Utility4, () => onEquipmentSelected(m_loadoutAsset.Utility4));

        m_passive1.Initialize(m_loadoutAsset.Passive1, () => onEquipmentSelected(m_loadoutAsset.Passive1));
        m_passive2.Initialize(m_loadoutAsset.Passive2, () => onEquipmentSelected(m_loadoutAsset.Passive2));
        m_passive3.Initialize(m_loadoutAsset.Passive3, () => onEquipmentSelected(m_loadoutAsset.Passive3));
        m_passive4.Initialize(m_loadoutAsset.Passive4, () => onEquipmentSelected(m_loadoutAsset.Passive4));

        if (getActiveInputDevice() != InputDeviceTypes.KeyboardAndMouse)
            EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, m_lArm.gameObject);
    }

    private void onEquipmentSelected(Equipment equipment)
    {
        if (equipment == null)
        {
            m_equipmentDataPanel.gameObject.SetActiveOptimized(false);
            return;
        }

        m_equipmentDataPanel.Clear();
        equipment.PopulateDataPanel(m_equipmentDataPanel);
        m_equipmentDataPanel.gameObject.SetActive(true);
    }

    public void Button_SlotClicked(int slotTypeInt)
    {
        var _slotType = (EquipmentSlotTypes)slotTypeInt;


    }
}