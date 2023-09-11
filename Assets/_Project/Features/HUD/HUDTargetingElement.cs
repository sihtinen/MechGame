using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDTargetingElement : PoolableBehaviour<HUDTargetingElement>
{
    [NonSerialized] public RectTransform RectTransformComponent = null;
    [NonSerialized] public Image ImageComponent = null;

    [Header("Object References")]
    [SerializeField] private TMP_Text m_camDotScoreValueText = null;
    [SerializeField] private TMP_Text m_compDotScoreValueText = null;
    [SerializeField] private TMP_Text m_distScoreValueText = null;
    [SerializeField] private TMP_Text m_totalScoreValueText = null;

    [SerializeField] private TMP_Text m_camDotScoreLabelText = null;
    [SerializeField] private TMP_Text m_compDotScoreLabelText = null;
    [SerializeField] private TMP_Text m_distScoreLabelText = null;
    [SerializeField] private TMP_Text m_totalScoreLabelText = null;

    protected override void resetAndClearBindings()
    {
        ImageComponent.enabled = false;

        m_camDotScoreValueText.enabled = false;
        m_compDotScoreValueText.enabled = false;
        m_distScoreValueText.enabled = false;
        m_totalScoreValueText.enabled = false;

        m_camDotScoreLabelText.enabled = false;
        m_compDotScoreLabelText.enabled = false;
        m_distScoreLabelText.enabled = false;
        m_totalScoreLabelText.enabled = false;
    }

    private void Awake()
    {
        RectTransformComponent = transform as RectTransform;

        TryGetComponent(out ImageComponent);
        ImageComponent.enabled = false;
    }

    public void BindToTargetingOption(MechTargeting.TargetingOption option, bool enableDebug)
    {
        //m_camDotScoreValueText.SetText(option.CameraDotScore.ToString("G2"));
        m_compDotScoreValueText.SetText(option.DotScore.ToString("G2"));
        m_distScoreValueText.SetText(option.DistScore.ToString("G2"));
        m_totalScoreValueText.SetText(option.TotalScore.ToString("G2"));

        //m_camDotScoreValueText.enabled = enableDebug;
        m_compDotScoreValueText.enabled = enableDebug;
        m_distScoreValueText.enabled = enableDebug;
        m_totalScoreValueText.enabled = enableDebug;

        //m_camDotScoreLabelText.enabled = enableDebug;
        m_compDotScoreLabelText.enabled = enableDebug;
        m_distScoreLabelText.enabled = enableDebug;
        m_totalScoreLabelText.enabled = enableDebug;
    }
}
