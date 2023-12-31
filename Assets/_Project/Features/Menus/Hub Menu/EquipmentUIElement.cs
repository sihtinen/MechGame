using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class EquipmentUIElement : PoolableBehaviour<EquipmentUIElement>
{
    [SerializeField] private TMP_Text m_slotNameText = null;
    [SerializeField] private TMP_Text m_nameText = null;
    [SerializeField] private TMP_Text m_typeText = null;

    private Equipment m_equipment = null;
    private UISelectionHighlight m_uiSelectionHighlight = null;
    private Action m_onSelectedCallback = null;
    private Action m_onClickedCallback = null;

    private Button m_button = null;

    private void Awake()
    {
        TryGetComponent(out m_button);
        m_button.onClick.AddListener(this.onButtonClicked);

        TryGetComponent(out m_uiSelectionHighlight);
        m_uiSelectionHighlight.OnSelected.AddListener(this.onSelected);
    }

    private void onButtonClicked()
    {
        m_onClickedCallback?.Invoke();
    }

    private void onSelected()
    {
        m_onSelectedCallback?.Invoke();
    }

    protected override void resetAndClearBindings()
    {
        m_equipment = null;
        m_onSelectedCallback = null;
        m_onClickedCallback = null;
    }

    public void Initialize(Equipment equipment, Action onSelectedCallback, Action onClickedCallback = null, EquipmentSlotTypes slotType = EquipmentSlotTypes.Undefined)
    {
        m_equipment = equipment;
        m_onSelectedCallback = onSelectedCallback;
        m_onClickedCallback = onClickedCallback;

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

    public void SetBottomLeftText(string text) => m_nameText.SetText(text);
    public void SetBottomRightText(string text) => m_typeText.SetText(text);
    public void SetTopLeftText(string text) => m_slotNameText.SetText(text);
    public void SetButtonInteractable(bool isInteractable) => m_button.interactable = isInteractable;
}

[System.Serializable]
public class EquipmentUIElementDictionary : SerializableDictionary<EquipmentSlotTypes, EquipmentUIElement> { }