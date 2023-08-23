using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EquipmentUIElement : PoolableBehaviour<EquipmentUIElement>
{
    [SerializeField] private TMP_Text m_slotNameText = null;
    [SerializeField] private TMP_Text m_nameText = null;
    [SerializeField] private TMP_Text m_typeText = null;

    private Equipment m_equipment = null;
    private UISelectionHighlight m_uiSelectionHighlight = null;
    private Action m_onSelectedCallback = null;

    private void Awake()
    {
        TryGetComponent(out m_uiSelectionHighlight);
        m_uiSelectionHighlight.OnSelected.AddListener(this.onSelected);
    }

    private void onSelected()
    {
        m_onSelectedCallback?.Invoke();
    }

    protected override void resetAndClearBindings()
    {
        m_equipment = null;
        m_onSelectedCallback = null;
    }

    public void Initialize(Equipment equipment, Action onSelectedCallback, EquipmentSlotTypes slotType = EquipmentSlotTypes.Undefined)
    {
        m_equipment = equipment;
        m_onSelectedCallback = onSelectedCallback;

        if (equipment != null)
        {
            m_nameText.SetText(m_equipment.DisplayName);
            m_typeText.SetText(m_equipment.Category.DisplayName);
        }
        else
        {
            m_nameText.SetText("-EMPTY-");
            m_typeText.SetText(string.Empty);
        }

        if (slotType != EquipmentSlotTypes.Undefined)
            m_slotNameText.SetText(slotType.ToString());
    }
}