using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Thruster Equipment")]
public class ThrusterEquipment : Equipment
{
    [Min(0f)] public float RechargeRate = 1.0f;

    [Header("Normal Thrust")]
    public bool EnableThrust_Normal = true;
    [Min(0f)] public float HorizontalForce_Normal;
    [Min(0f)] public float VerticalForce_Normal;
    [Min(0f)] public float EnergyDrainRate_Normal = 1f;

    [Header("Boost Thrust")]
    public bool EnableThrust_Boost = true;
    [Min(0f)] public float HorizontalForce_Boost;
    [Min(0f)] public float VerticalForce_Boost;
    [Min(0f)] public float EnergyDrainRate_Boost = 1f;

    [Header("HUD Settings")]
    public HUDThrusterEquipmentElement HUDPrefab = null;

    public override void InitializeGameplay(MechController mech, EquipmentSlotTypes slotType, bool isPlayer, InputActionReference inputActionRef)
    {
        var _thrusterComp = initializeRuntimeComponent<MechThrusterRuntime>(mech, slotType);

        if (isPlayer == false)
            return;

        _thrusterComp.BindToInput(inputActionRef);

        var _hudElement = HUDManager.Instance.InitializeHUDElement(HUDPrefab, slotType);
        _hudElement.BindToRuntimeComponent(_thrusterComp);
    }
}
