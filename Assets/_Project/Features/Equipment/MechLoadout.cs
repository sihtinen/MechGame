using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Mechs/New Mech Loadout")]
public class MechLoadout : ScriptableObject
{
    public string LoadoutName;

    [Space]

    public PrimaryEquipment LeftShoulder = null;
    public PrimaryEquipment RightShoulder = null;
    public PrimaryEquipment LeftArm = null;
    public PrimaryEquipment RightArm = null;

    public Equipment Melee = null;
    public Equipment Generator = null;

    public HeadEquipment Head = null;
    public ArmsEquipment Arms = null;
    public BodyEquipment Body = null;
    public LegsEquipment Legs = null;

    public UtilityEquipment Utility1 = null;
    public UtilityEquipment Utility2 = null;
    public UtilityEquipment Utility3 = null;
    public UtilityEquipment Utility4 = null;

    public PassiveEquipment Passive1 = null;
    public PassiveEquipment Passive2 = null;
    public PassiveEquipment Passive3 = null;
    public PassiveEquipment Passive4 = null;

    private static MechLoadoutSerialized m_cachedLoadoutSerialized = new();

    public MechLoadoutSerialized Serialize()
    {
        m_cachedLoadoutSerialized.LoadoutName = this.LoadoutName;

        m_cachedLoadoutSerialized.Slot_LeftShoulder_GUID = LeftShoulder != null ? LeftShoulder.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_RightShoulder_GUID = RightShoulder != null ? RightShoulder.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_LeftArm_GUID = LeftArm != null ? LeftArm.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_RightArm_GUID = RightArm != null ? RightArm.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Melee_GUID = Melee != null ? Melee.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Generator_GUID = Generator != null ? Generator.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Head_GUID = Head != null ? Head.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Arms_GUID = Arms != null ? Arms.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Body_GUID = Body != null ? Body.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Legs_GUID = Legs != null ? Legs.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Utility1_GUID = Utility1 != null ? Utility1.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Utility2_GUID = Utility2 != null ? Utility2.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Utility3_GUID = Utility3 != null ? Utility3.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Utility4_GUID = Utility4 != null ? Utility4.GUID.ToString() : null;

        m_cachedLoadoutSerialized.Slot_Passive1_GUID = Passive1 != null ? Passive1.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Passive2_GUID = Passive2 != null ? Passive2.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Passive3_GUID = Passive3 != null ? Passive3.GUID.ToString() : null;
        m_cachedLoadoutSerialized.Slot_Passive4_GUID = Passive4 != null ? Passive4.GUID.ToString() : null;

        return m_cachedLoadoutSerialized;
    }

    public void PopulateFromSerializedData(MechLoadoutSerialized loadoutSerialized)
    {
        var _equipmentDatabase = EquipmentDatabaseAccess.Instance.Database;

        LoadoutName = loadoutSerialized.LoadoutName;

        LeftShoulder = _equipmentDatabase.GetAsset<PrimaryEquipment>(loadoutSerialized.Slot_LeftShoulder_GUID);
        RightShoulder = _equipmentDatabase.GetAsset<PrimaryEquipment>(loadoutSerialized.Slot_RightShoulder_GUID);
        LeftArm = _equipmentDatabase.GetAsset<PrimaryEquipment>(loadoutSerialized.Slot_LeftArm_GUID);
        RightArm = _equipmentDatabase.GetAsset<PrimaryEquipment>(loadoutSerialized.Slot_RightArm_GUID);

        Melee = _equipmentDatabase.GetAsset<Equipment>(loadoutSerialized.Slot_Melee_GUID);
        Generator = _equipmentDatabase.GetAsset<Equipment>(loadoutSerialized.Slot_Generator_GUID);

        Head = _equipmentDatabase.GetAsset<HeadEquipment>(loadoutSerialized.Slot_Head_GUID);
        Arms = _equipmentDatabase.GetAsset<ArmsEquipment>(loadoutSerialized.Slot_Arms_GUID);
        Body = _equipmentDatabase.GetAsset<BodyEquipment>(loadoutSerialized.Slot_Body_GUID);
        Legs = _equipmentDatabase.GetAsset<LegsEquipment>(loadoutSerialized.Slot_Legs_GUID);

        Utility1 = _equipmentDatabase.GetAsset<UtilityEquipment>(loadoutSerialized.Slot_Utility1_GUID);
        Utility2 = _equipmentDatabase.GetAsset<UtilityEquipment>(loadoutSerialized.Slot_Utility2_GUID);
        Utility3 = _equipmentDatabase.GetAsset<UtilityEquipment>(loadoutSerialized.Slot_Utility3_GUID);
        Utility4 = _equipmentDatabase.GetAsset<UtilityEquipment>(loadoutSerialized.Slot_Utility4_GUID);

        Passive1 = _equipmentDatabase.GetAsset<PassiveEquipment>(loadoutSerialized.Slot_Passive1_GUID);
        Passive2 = _equipmentDatabase.GetAsset<PassiveEquipment>(loadoutSerialized.Slot_Passive2_GUID);
        Passive3 = _equipmentDatabase.GetAsset<PassiveEquipment>(loadoutSerialized.Slot_Passive3_GUID);
        Passive4 = _equipmentDatabase.GetAsset<PassiveEquipment>(loadoutSerialized.Slot_Passive4_GUID);
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