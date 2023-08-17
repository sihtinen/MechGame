using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Mechs/New Mech Loadout")]
public class MechLoadout : ScriptableObject
{
    public string LoadoutName;
    public Equipment Slot_LeftShoulder = null;
    public Equipment Slot_RightShoulder = null;
    public Equipment Slot_LeftArm = null;
    public Equipment Slot_RightArm = null;

    public MechLoadoutSerialized Serialize()
    {
        return new MechLoadoutSerialized
        {
            LoadoutName = this.LoadoutName,
            Slot_LeftShoulder_GUID = Slot_LeftShoulder != null ? Slot_LeftShoulder.GUID.ToString() : null,
            Slot_RightShoulder_GUID = Slot_RightShoulder != null ? Slot_RightShoulder.GUID.ToString() : null,
            Slot_LeftArm_GUID = Slot_LeftArm != null ? Slot_LeftArm.GUID.ToString() : null,
            Slot_RightArm_GUID = Slot_RightArm != null ? Slot_RightArm.GUID.ToString() : null,
        };
    }

    public void PopulateFromSerializedData(MechLoadoutSerialized loadoutSerialized)
    {
        LoadoutName = loadoutSerialized.LoadoutName;
    }

    [System.Serializable]
    public class MechLoadoutListSerialized
    {
        public List<MechLoadoutSerialized> AllLoadouts = new List<MechLoadoutSerialized>();
    }

    [System.Serializable]
    public class MechLoadoutSerialized
    {
        public string LoadoutName;
        public string Slot_LeftShoulder_GUID;
        public string Slot_RightShoulder_GUID;
        public string Slot_LeftArm_GUID;
        public string Slot_RightArm_GUID;
    }
}