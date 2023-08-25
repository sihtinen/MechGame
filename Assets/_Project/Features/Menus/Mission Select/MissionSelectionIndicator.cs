using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectionIndicator : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private bool m_animateWidth = false;
    [SerializeField] private bool m_animateHeight = false;
    [SerializeField, Min(0f)] private float m_minWidth = 0f;
    [SerializeField, Min(0f)] private float m_minHeight = 0f;
    [SerializeField] private float m_smoothTime = 1f;
    [SerializeField] private float m_maxSpeed = 1f;

    [Header("Object References")]
    [SerializeField] private MissionUIElement m_missionUIElement = null;

    private Vector2 m_defaultSize;
    private Vector2 m_velocity;
    private RectTransform m_rectTransform = null;

    private void Awake()
    {
        TryGetComponent(out m_rectTransform);
        m_defaultSize = m_rectTransform.GetSize();
    }

    private void OnDisable()
    {
        if (Application.isPlaying == false)
            return;

        var _targetSize = m_defaultSize;

        if (m_animateWidth)
            _targetSize.x = m_minWidth;
        if (m_animateHeight)
            _targetSize.y = m_minHeight;

        m_rectTransform.SetSize(_targetSize);
    }

    private void LateUpdate()
    {
        var _size = m_rectTransform.GetSize();
        var _targetSize = m_defaultSize;

        if (m_missionUIElement.IsSelected == false)
        {
            if (m_animateWidth)
                _targetSize.x = m_minWidth;
            if (m_animateHeight)
                _targetSize.y = m_minHeight;
        }

        _size = Vector2.SmoothDamp(_size, _targetSize, ref m_velocity, m_smoothTime, m_maxSpeed);
        m_rectTransform.SetSize(_size);
    }
}
