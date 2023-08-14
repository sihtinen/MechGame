using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    [NonSerialized] public Transform TransformComponent = null;
    public EquipmentSlotTypes SlotType = EquipmentSlotTypes.LeftShoulder;

    private void Awake()
    {
        TransformComponent = transform;
    }
}
