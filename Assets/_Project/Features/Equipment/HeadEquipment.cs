using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Head Equipment")]
public class HeadEquipment : Equipment
{
    public override void InitializeGameplay(MechController mech, EquipmentSlot slot, bool isPlayer, InputActionReference inputActionRef)
    {
        
    }
}
