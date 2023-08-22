using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutEditTab : UITab
{
    [SerializeField] private RectTransform m_scrollContentRoot = null;
    [SerializeField] private MechLoadout m_selectedLoadoutAsset = null;

    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new();
    private List<EquipmentSlotHeaderElement> m_activeSlotHeaderElements = new();
    private List<EquipmentUIElement> m_activeEquipmentElements = new();

    protected override void onOpened()
    {
        base.onOpened();

        rebuild();

        onActiveInputDeviceChanged(getActiveInputDevice());
    }

    protected override void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        if (IsOpened == false)
            return;

        rebuild();

        if (deviceType != InputDeviceTypes.KeyboardAndMouse)
            setFirstActiveChildAsSelected(m_scrollContentRoot);
    }

    private void rebuild()
    {
        for (int i = m_activeSlotHeaderElements.Count; i-- > 0;)
        {
            var _elem = m_activeSlotHeaderElements[i];
            _elem.ResetAndReturnToPool();
        }

        for (int i = m_activeEquipmentElements.Count; i --> 0;)
        {
            var _elem = m_activeEquipmentElements[i];
            _elem.ResetAndReturnToPool();
        }

        m_activeSlotHeaderElements.Clear();
        m_activeEquipmentElements.Clear();
        m_loadoutList.AllLoadouts.Clear();

        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        var _loadoutDataSerialized = m_loadoutList.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex];
        m_selectedLoadoutAsset.PopulateFromSerializedData(_loadoutDataSerialized);

        createElements(m_selectedLoadoutAsset.Slot_RightShoulder, EquipmentSlotTypes.RightShoulder);
        createElements(m_selectedLoadoutAsset.Slot_LeftShoulder, EquipmentSlotTypes.LeftShoulder);
        createElements(m_selectedLoadoutAsset.Slot_RightArm, EquipmentSlotTypes.RightArm);
        createElements(m_selectedLoadoutAsset.Slot_LeftArm, EquipmentSlotTypes.LeftArm);
    }

    private void createElements(Equipment equipment, EquipmentSlotTypes slotType)
    {
        var _newSlotHeaderElement = EquipmentSlotHeaderElementPool.Get();
        _newSlotHeaderElement.SetText(slotType.ToString());
        _newSlotHeaderElement.transform.SetParent(m_scrollContentRoot);
        _newSlotHeaderElement.gameObject.SetActiveOptimized(true);
        m_activeSlotHeaderElements.Add(_newSlotHeaderElement);

        var _equipmentElement = EquipmentUIElementPool.Get();
        _equipmentElement.Initialize(equipment, slotType);
        _equipmentElement.transform.SetParent(m_scrollContentRoot);
        _equipmentElement.gameObject.SetActiveOptimized(true);
        m_activeEquipmentElements.Add(_equipmentElement);
    }
}