using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicHUDTarget : SingletonBehaviour<DynamicHUDTarget>
{
    public enum HUDDirection
    {
        LocalForward = 0,
        LocalUp = 1,
        LocalRight = 2,

        LocalBack = 3,
        LocalDown = 4,
        LocalLeft = 5,
    }

    [Header("Dynamic HUD Settings")]
    [SerializeField] private HUDDirection m_facingDirectionType = HUDDirection.LocalForward;
    [SerializeField] private Rigidbody m_rigidbody = null;

    public Vector3 GetFacingDirection()
    {
        switch (m_facingDirectionType)
        {
            default:
            case HUDDirection.LocalForward:
                return transform.forward;
            case HUDDirection.LocalUp:
                return transform.up;
            case HUDDirection.LocalRight:
                return transform.right;

            case HUDDirection.LocalBack:
                return -transform.forward;
            case HUDDirection.LocalDown:
                return -transform.up;
            case HUDDirection.LocalLeft:
                return -transform.right;
        }
    }

    public Vector3 GetVelocity()
    {
        return m_rigidbody.velocity;
    }
}