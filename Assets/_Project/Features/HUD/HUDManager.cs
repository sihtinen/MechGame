using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : SingletonBehaviour<HUDManager>
{
    [Header("Object References")]
    [SerializeField] private List<HUDEquipmentSlot> m_hudEquipmentSlots = new List<HUDEquipmentSlot>();

    public HUDEquipmentElementBase InitializeHUDElement(HUDEquipmentElementBase hudElementPrefab, EquipmentSlotTypes slotType)
    {
        var _targetSlot = getSlot(slotType);

        if (_targetSlot == null)
        {
            Debug.LogError($"HUDManager.InitializeHUDElement(): returning null, no slots found for type: " + slotType);
            return null;
        }

        var _newObj = Instantiate(hudElementPrefab.gameObject);
        _newObj.TryGetComponent(out HUDEquipmentElementBase _hudElement);
        _hudElement.Initialize(_targetSlot);
        return _hudElement;
    }

    private HUDEquipmentSlot getSlot(EquipmentSlotTypes slotType)
    {
        for (int i = 0; i < m_hudEquipmentSlots.Count; i++)
        {
            var _slot = m_hudEquipmentSlots[i];

            if (_slot.SlotType == slotType)
                return _slot;
        }

        return null;
    }
}
