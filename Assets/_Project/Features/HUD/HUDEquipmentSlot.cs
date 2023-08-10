using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDEquipmentSlot : MonoBehaviour
{
    public EquipmentSlotTypes SlotType = EquipmentSlotTypes.LeftShoulder;

    [NonSerialized] public RectTransform RectTransformComponent = null;

    private void Awake()
    {
        TryGetComponent(out RectTransformComponent);
    }
}
