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

    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new();

    protected override void onOpened()
    {
        base.onOpened();

        rebuild();
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


    }
}