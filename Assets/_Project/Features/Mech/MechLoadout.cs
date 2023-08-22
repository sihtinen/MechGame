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

    public Equipment Slot_Melee = null;
    public Equipment Slot_Generator = null;

    public Equipment Slot_Head = null;
    public Equipment Slot_Arms = null;
    public Equipment Slot_Body = null;
    public Equipment Slot_Legs = null;

    public Equipment Slot_Utility1 = null;
    public Equipment Slot_Utility2 = null;
    public Equipment Slot_Utility3 = null;
    public Equipment Slot_Utility4 = null;

    public Equipment Slot_Passive1 = null;
    public Equipment Slot_Passive2 = null;
    public Equipment Slot_Passive3 = null;
    public Equipment Slot_Passive4 = null;

    private static MechLoadoutSerialized m_cachedLoadoutSerialized = new();

    public MechLoadoutSerialized Serialize()
    {
        m_cachedLoadoutSerialized.LoadoutName = this.LoadoutName;

        m_cachedLoadoutSerialized.Slot_LeftShoulder_GUID = Slot_LeftShoulder != null ? Slot_LeftShoulder.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_RightShoulder_GUID = Slot_RightShoulder != null ? Slot_RightShoulder.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_LeftArm_GUID = Slot_LeftArm != null ? Slot_LeftArm.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_RightArm_GUID = Slot_RightArm != null ? Slot_RightArm.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Melee_GUID = Slot_Melee != null ? Slot_Melee.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Generator_GUID = Slot_Generator != null ? Slot_Generator.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Head_GUID = Slot_Head != null ? Slot_Head.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Arms_GUID = Slot_Arms != null ? Slot_Arms.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Body_GUID = Slot_Body != null ? Slot_Body.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Legs_GUID = Slot_Legs != null ? Slot_Legs.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Utility1_GUID = Slot_Utility1 != null ? Slot_Utility1.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Utility2_GUID = Slot_Utility2 != null ? Slot_Utility2.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Utility3_GUID = Slot_Utility3 != null ? Slot_Utility3.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Utility4_GUID = Slot_Utility4 != null ? Slot_Utility4.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Passive1_GUID = Slot_Passive1 != null ? Slot_Passive1.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Passive2_GUID = Slot_Passive2 != null ? Slot_Passive2.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Passive3_GUID = Slot_Passive3 != null ? Slot_Passive3.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Passive4_GUID = Slot_Passive4 != null ? Slot_Passive4.GUID.ToString() : null;

        return m_cachedLoadoutSerialized;
    }

    public void PopulateFromSerializedData(MechLoadoutSerialized loadoutSerialized)
    {
        var _equipmentDatabase = EquipmentDatabaseAccess.Instance.Database;

        LoadoutName = loadoutSerialized.LoadoutName;
        Slot_LeftShoulder = _equipmentDatabase.GetAsset(loadoutSerialized.Slot_LeftShoulder_GUID);
        Slot_RightShoulder = _equipmentDatabase.GetAsset(loadoutSerialized.Slot_RightShoulder_GUID);
        Slot_LeftArm = _equipmentDatabase.GetAsset(loadoutSerialized.Slot_LeftArm_GUID);
        Slot_RightArm = _equipmentDatabase.GetAsset(loadoutSerialized.Slot_RightArm_GUID);
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

        public string Slot_Melee_GUID;
        public string Slot_Generator_GUID;

        public string Slot_Head_GUID;
        public string Slot_Arms_GUID;
        public string Slot_Body_GUID;
        public string Slot_Legs_GUID;

        public string Slot_Utility1_GUID;
        public string Slot_Utility2_GUID;
        public string Slot_Utility3_GUID;
        public string Slot_Utility4_GUID;

        public string Slot_Passive1_GUID;
        public string Slot_Passive2_GUID;
        public string Slot_Passive3_GUID;
        public string Slot_Passive4_GUID;
    }
}