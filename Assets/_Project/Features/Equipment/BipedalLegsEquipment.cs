using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/Legs/New Bipedal Legs")]
public class BipedalLegsEquipment : LegsEquipment
{
    public override void InitializeGameplay(MechController mech, EquipmentSlot slot, bool isPlayer, InputActionReference inputActionRef)
    {

    }
}