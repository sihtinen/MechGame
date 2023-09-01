using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class FreeAimCrosshair : SingletonBehaviour<FreeAimCrosshair>
{
    [SerializeField] private float m_smoothDampTime = 0.1f;
    [SerializeField] private float m_smoothDampMaxSpeed = 50f;
    [SerializeField] private float m_borderBufferSize_X = 60f;
    [SerializeField] private float m_borderBufferSize_Y = 40f;
    [SerializeField] private RectTransform m_target = null;

    private RectTransform m_rectTransform = null;
    private RectTransform m_parentTransform = null;

    private void Start()
    {
        m_rectTransform = transform as RectTransform;
        m_parentTransform = transform.parent as RectTransform;
    }

    private void LateUpdate()
    {
        float _parentWidth = m_parentTransform.GetWidth();
        float _parentHeight = m_parentTransform.GetHeight();

        float _posLimitX_Min = m_parentTransform.position.x + m_borderBufferSize_X - 0.5f * _parentWidth;
        float _posLimitX_Max = m_parentTransform.position.x - m_borderBufferSize_X + 0.5f * _parentWidth;

        float _posLimitY_Min = m_parentTransform.position.y + m_borderBufferSize_Y - 0.5f * _parentHeight;
        float _posLimitY_Max = m_parentTransform.position.y - m_borderBufferSize_Y + 0.5f * _parentHeight;

        Vector3 _targetPos = m_target.position;

        _targetPos.x = Mathf.Clamp(_targetPos.x,
            _posLimitX_Min,
            _posLimitX_Max);

        _targetPos.y = Mathf.Clamp(_targetPos.y,
            _posLimitY_Min,
            _posLimitY_Max);

        m_rectTransform.position = Vector3.Lerp(m_rectTransform.position, _targetPos, Time.deltaTime * 30f);

        var _clampedPos = m_rectTransform.anchoredPosition;

        _clampedPos.x = Mathf.Clamp(_clampedPos.x,
            m_borderBufferSize_X,
            _parentWidth - m_borderBufferSize_X);

        _clampedPos.y = Mathf.Clamp(_clampedPos.y,
            m_borderBufferSize_Y,
            _parentHeight - m_borderBufferSize_Y);

        m_rectTransform.anchoredPosition = _clampedPos;
        m_rectTransform.rotation = Quaternion.identity;
    }
}
