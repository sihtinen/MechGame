using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUIElement : MonoBehaviour
{
    [Header("Runtime Parameters")]
    [NonEditable] public bool IsHovered = false;
    [NonEditable] public bool IsSelected = false;

    [Header("Mission Data Asset")]
    public MissionData Mission = null;

    [Header("Navigation")]
    public MissionUIElement ConnectionLeft = null;
    public MissionUIElement ConnectionRight = null;
    public MissionUIElement ConnectionUp = null;
    public MissionUIElement ConnectionDown = null;

    [Header("Object References")]
    [SerializeField] private UISelectionHighlight m_highlightComponent = null;
    [SerializeField] private Button m_button = null;
    [SerializeField] private Image m_missionIconImage = null;
    [SerializeField] private TMP_Text m_missionNameText = null;

    private Color m_missionIconColor;

    private void Awake()
    {
        m_missionIconColor = m_missionIconImage.color;

        if (Mission != null)
        {
            m_missionNameText.SetText(Mission.DisplayName);
        }

        m_highlightComponent.OnSelected.AddListener(this.onHoverStart);
        m_highlightComponent.OnDeselected.AddListener(this.onHoverEnd);
        m_button.onClick.AddListener(this.onClicked);

        ObjectCollection<MissionUIElement>.RegisterObject(this);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying == false)
            return;

        ObjectCollection<MissionUIElement>.UnregisterObject(this);

        if (m_highlightComponent != null)
        {
            m_highlightComponent.OnSelected?.RemoveAllListeners();
            m_highlightComponent.OnDeselected?.RemoveAllListeners();
        }

        if (m_button != null)
            m_button.onClick?.RemoveAllListeners();
    }

    public void onHoverStart()
    {
        IsHovered = true;
        m_missionIconImage.color = m_missionIconColor + new Color(0.4f, 0.4f, 0.4f);

        MissionSelectScreen.Instance.SetHighlightMission(this);
    }

    public void onHoverEnd()
    {
        IsHovered = false;
        m_missionIconImage.color = m_missionIconColor;
    }

    private void onClicked()
    {
        MissionSelectScreen.Instance.SetActiveMission(this);
    }
}
