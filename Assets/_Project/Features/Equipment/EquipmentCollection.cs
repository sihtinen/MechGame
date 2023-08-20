using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Equipment Collection")]
public class EquipmentCollection : ScriptableObject
{
    public List<Equipment> EquipmentAssets = new List<Equipment>();

    private EquipmentCollectionSerialized m_serializedCollection = new();

    public EquipmentCollectionSerialized Serialize()
    {
        m_serializedCollection.EquipmentGUIDs.Clear();

        for (int i = 0; i < EquipmentAssets.Count; i++)
        {
            var _asset = EquipmentAssets[i];
            m_serializedCollection.EquipmentGUIDs.Add(_asset.GUID.ToString());
        }

        return m_serializedCollection;
    }

    public void PopulateFromSerializedData(EquipmentCollectionSerialized data)
    {
        EquipmentAssets.Clear();

        var _equipmentDatabase = EquipmentDatabaseAccess.Instance.Database;

        for (int i = 0; i < data.EquipmentGUIDs.Count; i++)
        {
            var _guid = data.EquipmentGUIDs[i];
            var _asset = _equipmentDatabase.GetAsset(_guid);

            if (_asset != null)
                EquipmentAssets.Add(_asset);
        }
    }

    [System.Serializable]
    public class EquipmentCollectionSerialized
    {
        public List<string> EquipmentGUIDs = new List<string>();
    }
}
