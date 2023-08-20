using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTab : UITab
{
    [Header("Object References")]
    [SerializeField] private RectTransform m_verticalGroupContent = null;
    [SerializeField] private EquipmentCollection m_equipmentCollection = null;

    private EquipmentCollection.EquipmentCollectionSerialized m_equipmentCollectionSerialized = new();
    private List<EquipmentTypeHeaderElement> m_activeCategoryElements = new();
    private List<EquipmentUIElement> m_activeUIElements = new();
    private Dictionary<EquipmentCategory, List<Equipment>> m_equipmentCategoryCollection = new();

    protected override void onOpened()
    {
        base.onOpened();

        rebuild();
    }

    private void rebuild()
    {
        clearOldData();

        fetchEquipmentData();

        for (int i = 0; i < m_equipmentCollection.EquipmentAssets.Count; i++)
        {
            var _equipment = m_equipmentCollection.EquipmentAssets[i];

            if (m_equipmentCategoryCollection.ContainsKey(_equipment.Category) == false)
                m_equipmentCategoryCollection.Add(_equipment.Category, new List<Equipment>());

            m_equipmentCategoryCollection[_equipment.Category].Add(_equipment);

        }

        foreach (var _kvp in m_equipmentCategoryCollection)
        {
            var _categoryHeader = EquipmentTypeHeaderElementPool.Get();
            _categoryHeader.SetText(_kvp.Key.DisplayName);
            _categoryHeader.transform.SetParent(m_verticalGroupContent);
            _categoryHeader.gameObject.SetActiveOptimized(true);

            m_activeCategoryElements.Add(_categoryHeader);

            for (int i = 0; i < _kvp.Value.Count; i++)
            {
                var _equipment = _kvp.Value[i];

                var _newUI = EquipmentUIElementPool.Get();
                _newUI.transform.SetParent(m_verticalGroupContent);
                _newUI.BindToEquipment(_equipment);
                _newUI.gameObject.SetActiveOptimized(true);

                m_activeUIElements.Add(_newUI);
            }
        }
    }

    private void clearOldData()
    {
        for (int i = m_activeCategoryElements.Count; i-- > 0;)
            m_activeCategoryElements[i].ResetAndReturnToPool();

        for (int i = m_activeUIElements.Count; i-- > 0;)
            m_activeUIElements[i].ResetAndReturnToPool();

        m_activeCategoryElements.Clear();
        m_activeUIElements.Clear();
        m_equipmentCategoryCollection.Clear();
    }

    private void fetchEquipmentData()
    {
        m_equipmentCollectionSerialized.EquipmentGUIDs.Clear();

        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.UNLOCKED_EQUIPMENT_ID, ref m_equipmentCollectionSerialized);

        m_equipmentCollection.PopulateFromSerializedData(m_equipmentCollectionSerialized);
    }
}