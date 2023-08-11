using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarTick : MonoBehaviour
{
    [SerializeField] private Gradient m_colorGradient = new Gradient();
    [SerializeField] private Image m_background = null;
    [SerializeField] private Image m_healthIndicator = null;

    private HealthBarTickState m_state = HealthBarTickState.Alive;
    private float m_destroyedTime = 0f;

    private void Awake()
    {
        SetState(HealthBarTickState.Invisible, instantChange: true);
    }

    public void SetState(HealthBarTickState state, bool instantChange = false)
    {
        if (m_state == state)
            return;

        m_state = state;

        switch (m_state)
        {
            default:
            case HealthBarTickState.Invisible:
                m_background.enabled = false;
                m_healthIndicator.enabled = false;
                m_healthIndicator.color = Color.white;
                break;

            case HealthBarTickState.Destroyed:
                m_destroyedTime = instantChange ? 1 : 0;
                break;

            case HealthBarTickState.Alive:
                m_background.enabled = true;
                m_healthIndicator.enabled = true;
                break;
        }
    }

    public void ManualUpdate(float deltaTime)
    {
        switch (m_state)
        {
            default:
            case HealthBarTickState.Invisible:
                break;

            case HealthBarTickState.Destroyed:
                
                if (m_destroyedTime < 1f)
                {
                    m_destroyedTime += deltaTime * 4;

                    m_healthIndicator.rectTransform.localScale = (1f + m_destroyedTime * 4f) * Vector3.one;

                    var _col = m_healthIndicator.color;
                    _col.a = (1f - m_destroyedTime);
                    m_healthIndicator.color = _col;

                    if (m_destroyedTime >= 1f)
                        m_healthIndicator.enabled = false;
                }

                break;

            case HealthBarTickState.Alive:
                break;
        }
    }

    public void SetHealthNormalized(float healthNormalized)
    {
        m_healthIndicator.color = m_colorGradient.Evaluate(1f - healthNormalized);
    }

    public enum HealthBarTickState
    {
        Invisible,
        Destroyed,
        Alive,
    }
}
