using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Mechs/New Mech Loadout")]
public class MechLoadout : ScriptableObject
{
    public Equipment Slot_LeftShoulder = null;
    public Equipment Slot_RightShoulder = null;
    public Equipment Slot_LeftArm = null;
    public Equipment Slot_RightArm = null;
}