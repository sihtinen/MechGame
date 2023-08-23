using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Mechs/New Mech Loadout")]
public class MechLoadout : ScriptableObject
{
    public string LoadoutName;

    public EquipmentSlotDictionary Dictionary = new();

    private static MechLoadoutSerialized m_cachedLoadoutSerialized = new();

    public MechLoadoutSerialized Serialize()
    {
        m_cachedLoadoutSerialized.LoadoutName = this.LoadoutName;

        m_cachedLoadoutSerialized.DictionarySerialized.Clear();

        foreach (var _kvp in Dictionary)
        {
            m_cachedLoadoutSerialized.DictionarySerialized.Add(
                _kvp.Key, 
                _kvp.Value != null ? _kvp.Value.GUID.ToString() : null);
        }

        return m_cachedLoadoutSerialized;
    }

    public void PopulateFromSerializedData(MechLoadoutSerialized loadoutSerialized)
    {
        var _equipmentDatabase = EquipmentDatabaseAccess.Instance.Database;

        Dictionary.Clear();

        LoadoutName = loadoutSerialized.LoadoutName;

        foreach (var _kvp in loadoutSerialized.DictionarySerialized)
            Dictionary.Add(_kvp.Key, _equipmentDatabase.GetAsset<Equipment>(_kvp.Value));
    }

    [ContextMenu("Clear Loadout")]
    public void ClearLoadout()
    {
        Dictionary.Clear();

        foreach (int i in Enum.GetValues(typeof(EquipmentSlotTypes)))
        {
            if (i < 0)
                continue;

            Dictionary.Add((EquipmentSlotTypes)i, null);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
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
        public EquipmentSlotDictionarySerialized DictionarySerialized = new();
    }
}

[System.Serializable]
public class EquipmentSlotDictionary : SerializableDictionary<EquipmentSlotTypes, Equipment> { }

[System.Serializable]
public class EquipmentSlotDictionarySerialized : SerializableDictionary<EquipmentSlotTypes, string> { }