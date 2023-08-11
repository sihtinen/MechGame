using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : PoolableBehaviour<HealthBar>
{
    [SerializeField] private Vector3 m_anchorOffset = new Vector3(-0.1f, 0f);

    [Header("Object References")]
    [SerializeField] private HealthBarTick m_tickPrefab = null;

    private float m_becameVisibleTime = float.MinValue;

    private RectTransform m_rectTransform = null;
    private IDamageable m_binding = null;
    private CanvasGroup m_canvasGroup = null;
    private List<HealthBarTick> m_ticks = new List<HealthBarTick>();

    private static readonly int HEALTH_PER_TICK = 250;
    private static readonly float VISIBLE_TIME = 5f;

    private void Awake()
    {
        m_rectTransform = transform as RectTransform;
        TryGetComponent(out m_canvasGroup);

        var _anchorPos = new Vector3(-2, -2, 0);
        m_rectTransform.anchorMin = _anchorPos;
        m_rectTransform.anchorMax = _anchorPos;
        m_rectTransform.anchoredPosition = Vector3.zero;
    }

    protected override void resetAndClearBindings()
    {
        if (m_binding != null)
        {
            m_binding.OnHealthUpdated -= this.onHealthUpdated;
            m_binding = null;
        }
    }

    public void BindToInterface(IDamageable damageable)
    {
        m_binding = damageable;
        m_binding.OnHealthUpdated += this.onHealthUpdated;

        int _targetTickCount = m_binding.GetMaxHealth() / HEALTH_PER_TICK;

        if (m_ticks.Count < _targetTickCount)
        {
            while (m_ticks.Count < _targetTickCount)
            {
                var _newTickObj = Instantiate(m_tickPrefab.gameObject, parent: m_rectTransform);
                _newTickObj.TryGetComponent(out HealthBarTick _tick);
                _tick.SetHealthNormalized(1f);
                m_ticks.Add(_tick);
            }
        }

        int _aliveTickCount = (m_binding.GetCurrentHealth() / HEALTH_PER_TICK) + 1;

        for (int i = 0; i < _targetTickCount; i++)
        {
            var _tick = m_ticks[i];

            if (i < _aliveTickCount)
                _tick.SetState(HealthBarTick.HealthBarTickState.Alive, instantChange: true);
            else
                _tick.SetState(HealthBarTick.HealthBarTickState.Destroyed, instantChange: true);
        }
    }

    private void onHealthUpdated(IDamageable.HealthUpdateParams healthParams)
    {
        m_becameVisibleTime = Time.time;

        int _currentHealth = m_binding.GetCurrentHealth();
        int _targetTickCount = m_binding.GetMaxHealth() / HEALTH_PER_TICK;

        if (_currentHealth <= 0)
        {
            for (int i = 0; i < m_ticks.Count; i++)
            {
                if (i < _targetTickCount)
                    m_ticks[i].SetState(HealthBarTick.HealthBarTickState.Destroyed);
            }

            return;
        }

        int _aliveTickCount = (_currentHealth / HEALTH_PER_TICK) + 1;
        int _healthRemainder = _currentHealth % HEALTH_PER_TICK;

        int _currentTickIndex = _aliveTickCount - 1;
        m_ticks[_currentTickIndex].SetHealthNormalized((float)_healthRemainder / HEALTH_PER_TICK);

        for (int i = 0; i < m_ticks.Count; i++)
        {
            if (i > _currentTickIndex && i < _targetTickCount)
                m_ticks[i].SetState(HealthBarTick.HealthBarTickState.Destroyed);
        }
    }

    private void LateUpdate()
    {
        if (m_binding == null)
            return;

        if (m_binding.GetCurrentHealth() <= 0)
        {
            if (m_canvasGroup.alpha > 0)
                m_canvasGroup.alpha -= Time.deltaTime * 2;
        }

        float _timeSinceBecameVisible = Time.time - m_becameVisibleTime;
        bool _isVisible = _timeSinceBecameVisible < VISIBLE_TIME;

        Vector3 _anchorPos;

        if (_isVisible == false)
        {
            _anchorPos = new Vector3(-2, -2, 0);
            m_rectTransform.anchorMin = _anchorPos;
            m_rectTransform.anchorMax = _anchorPos;
            m_rectTransform.anchoredPosition = Vector3.zero;
            return;
        }

        var _mainCam = MainCameraComponent.Instance.CameraComponent;
        _anchorPos = _mainCam.WorldToViewportPoint(m_binding.GetTransform().position);

        if (_anchorPos.z < 0)
            _anchorPos = new Vector3(-2, -2, 0);
        else
            _anchorPos += m_anchorOffset;

        m_rectTransform.anchorMin = _anchorPos;
        m_rectTransform.anchorMax = _anchorPos;
        m_rectTransform.anchoredPosition = Vector3.zero;

        float _deltaTime = Time.deltaTime;

        for (int i = 0; i < m_ticks.Count; i++)
            m_ticks[i].ManualUpdate(_deltaTime);
    }
}
