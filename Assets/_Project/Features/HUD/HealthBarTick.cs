using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarTick : MonoBehaviour
{
    [SerializeField] private Image m_background = null;
    [SerializeField] private Image m_healthIndicator = null;

    private void Awake()
    {
        SetState(HealthBarTickState.Invisible);
    }

    public void SetState(HealthBarTickState state)
    {
        switch (state)
        {
            default:
            case HealthBarTickState.Invisible:
                m_background.enabled = false;
                m_healthIndicator.enabled = false;
                break;

            case HealthBarTickState.Destroyed:
                m_background.enabled = true;
                m_healthIndicator.enabled = false;
                break;

            case HealthBarTickState.Alive:
                m_background.enabled = true;
                m_healthIndicator.enabled = true;
                break;
        }
    }

    public enum HealthBarTickState
    {
        Invisible,
        Destroyed,
        Alive,
    }
}
