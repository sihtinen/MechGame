using Tensori.SaveSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Save Data/New Default Unlocks Processor")]
public class SaveDefaultUnlocksProcessor : SavePreProcessor
{
    [SerializeField] private EquipmentCollection m_defaultUnlocksCollection = null;

    private static EquipmentCollection.EquipmentCollectionSerialized m_equipmentCollectionSerialized = new();

    public override void PreProcess(SaveData saveData)
    {
        m_equipmentCollectionSerialized.EquipmentGUIDs.Clear();

        bool _dataFound = saveData.ReadObject(SaveIDConstants.UNLOCKED_EQUIPMENT_ID, ref m_equipmentCollectionSerialized);
        bool _requireUpdate = _dataFound == false;

        var _defaultUnlocksSerialized = m_defaultUnlocksCollection.Serialize();

        for (int i = 0; i < _defaultUnlocksSerialized.EquipmentGUIDs.Count; i++)
        {
            var _guid = _defaultUnlocksSerialized.EquipmentGUIDs[i];

            if (m_equipmentCollectionSerialized.EquipmentGUIDs.Contains(_guid) == false)
            {
                m_equipmentCollectionSerialized.EquipmentGUIDs.Add(_guid);
                _requireUpdate = true;
            }
        }

        if (_requireUpdate)
            saveData.RegisterVariable(SaveIDConstants.UNLOCKED_EQUIPMENT_ID, m_equipmentCollectionSerialized);
    }
}
