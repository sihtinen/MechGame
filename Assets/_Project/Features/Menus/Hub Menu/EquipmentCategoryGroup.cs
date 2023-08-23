using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCategoryGroup : PoolableBehaviour<EquipmentCategoryGroup>
{
    private RectTransform m_rectTransform = null;
    private List<EquipmentUIElement> m_activeElements = new();

    private Action<Equipment> m_onEquipmentSelectedCallback = null;
    private Action<Equipment> m_onEquipmentClickedCallback = null;

    private void Awake()
    {
        m_rectTransform = transform as RectTransform;
    }

    protected override void resetAndClearBindings()
    {
        m_onEquipmentSelectedCallback = null;
        m_onEquipmentClickedCallback = null;

        for (int i = m_activeElements.Count; i --> 0;)
            m_activeElements[i].ResetAndReturnToPool();

        m_activeElements.Clear();
    }

    public void Populate(
        EquipmentCategory category,
        EquipmentCollection equipmentCollection,
        Equipment currentlyEquippedAsset,
        Action<Equipment> onEquipmentSelectedCallback,
        Action<Equipment> onEquipmentClickedCallback)
    {
        m_onEquipmentSelectedCallback = onEquipmentSelectedCallback;
        m_onEquipmentClickedCallback = onEquipmentClickedCallback;

        if (category == null)
        {
            var _emptyElement = EquipmentUIElementPool.Get();
            _emptyElement.transform.SetParent(transform);

            _emptyElement.Initialize(
                null, 
                onSelectedCallback: () => onEquipmentSelected(null), 
                onClickedCallback: () => onEquipmentClicked(null));

            _emptyElement.SetButtonInteractable(currentlyEquippedAsset != null);
            _emptyElement.SetTopLeftText(string.Empty);
            _emptyElement.SetBottomRightText(string.Empty);
            _emptyElement.gameObject.SetActiveOptimized(true);

            m_activeElements.Add(_emptyElement);

            return;
        }

        for (int i = 0; i < equipmentCollection.EquipmentAssets.Count; i++)
        {
            var _asset = equipmentCollection.EquipmentAssets[i];
            if (_asset.Category == category)
            {
                var _uiElement = EquipmentUIElementPool.Get();
                _uiElement.transform.SetParent(transform);

                _uiElement.Initialize(
                    _asset, 
                    onSelectedCallback: () => onEquipmentSelected(_asset),
                    onClickedCallback: () => onEquipmentClicked(_asset));

                _uiElement.gameObject.SetActiveOptimized(true);
                _uiElement.SetButtonInteractable(currentlyEquippedAsset != _asset);
                m_activeElements.Add(_uiElement);
            }
        }

        int _height = 70;
        int _counter = 0;

        for (int i = 0; i < m_activeElements.Count; i++)
        {
            _counter++;

            if (_counter > 2)
            {
                _counter -= 2;
                _height += 70;
            }
        }

        m_rectTransform.SetHeight(_height);
    }

    private void onEquipmentSelected(Equipment asset)
    {
        m_onEquipmentSelectedCallback?.Invoke(asset);
    }

    private void onEquipmentClicked(Equipment asset)
    {
        m_onEquipmentClickedCallback?.Invoke(asset);
    }
}
