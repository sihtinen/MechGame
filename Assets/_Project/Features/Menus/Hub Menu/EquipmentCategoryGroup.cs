using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCategoryGroup : PoolableBehaviour<EquipmentCategoryGroup>
{
    private RectTransform m_rectTransform = null;
    private List<EquipmentUIElement> m_activeElements = new();

    private void Awake()
    {
        m_rectTransform = transform as RectTransform;
    }

    protected override void resetAndClearBindings()
    {
        for (int i = m_activeElements.Count; i --> 0;)
            m_activeElements[i].ResetAndReturnToPool();

        m_activeElements.Clear();
    }

    public void Populate(EquipmentCategory category, EquipmentCollection equipmentCollection)
    {
        for (int i = 0; i < equipmentCollection.EquipmentAssets.Count; i++)
        {
            var _asset = equipmentCollection.EquipmentAssets[i];
            if (_asset.Category == category)
            {
                var _uiElement = EquipmentUIElementPool.Get();
                _uiElement.transform.SetParent(transform);
                _uiElement.Initialize(_asset, onSelectedCallback: null);
                _uiElement.gameObject.SetActiveOptimized(true);
                m_activeElements.Add(_uiElement);
            }
        }

        int _height = 70;
        int _counter = 0;

        for (int i = 0; i < m_activeElements.Count; i++)
        {
            _counter++;

            if (_counter == 2)
            {
                _counter = 0;
                _height += 70;
            }
        }

        m_rectTransform.SetHeight(_height);
    }
}
