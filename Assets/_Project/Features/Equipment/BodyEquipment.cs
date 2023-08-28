using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Body Equipment")]
public class BodyEquipment : Equipment
{
    [Header("Rotation Settings")]
    [Min(0)] public int MaxRotationAngle_Yaw = 45;
    [Min(0)] public float RotationSmoothTime_Yaw = 0.5f;
    [Min(0)] public float RotationMaxSpeed_Yaw = 0.5f;

    public override void InitializeGameplay(MechController mech, EquipmentSlot slot, bool isPlayer, InputActionReference inputActionRef)
    {

    }
}