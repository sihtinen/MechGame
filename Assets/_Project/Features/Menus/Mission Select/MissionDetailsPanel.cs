using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionDetailsPanel : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_nameText = null;
    [SerializeField] private TMP_Text m_descriptionText = null;

    [NonSerialized] public RectTransform RectTransformComponent = null;

    private void Awake()
    {
        RectTransformComponent = transform as RectTransform;
    }

    public void Populate(MissionData mission)
    {
        m_nameText.SetText(mission.DisplayName);
        m_descriptionText.SetText(mission.Description);
    }
}
