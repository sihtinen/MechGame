using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RigidBodyEntity : MonoBehaviour
{
    [NonSerialized] public Transform TransformComponent = null;
    [NonSerialized] public Rigidbody RigidBody = null;

    protected float m_time;
    protected float m_deltaTime;

    protected Vector3 m_upDirection;
    protected Vector3 m_forwardDirection;
    protected Vector3 m_rightDirection;

    protected virtual void Awake()
    {
        TransformComponent = transform;
        TryGetComponent(out RigidBody);
    }

    protected virtual void FixedUpdate()
    {
        m_time = Time.time;
        m_deltaTime = Time.fixedDeltaTime;
        m_upDirection = TransformComponent.up;
        m_forwardDirection = TransformComponent.forward;
        m_rightDirection = TransformComponent.right;
    }

    protected void applyForcePID(PID pid, ref PID.PIDState pidState, Vector3 forceDirectionLocal, float currentValue, float targetValue)
    {
        pidState = pid.UpdateTick(m_deltaTime, pidState, currentValue, targetValue);
        Vector3 _force = m_deltaTime * pidState.Output * forceDirectionLocal;
        RigidBody.AddRelativeForce(_force, ForceMode.Acceleration);
    }

    protected void applyTorquePID(PID pid, ref PID.PIDState pidState, Vector3 rotateAxisLocal, float currentValue, float targetValue)
    {
        pidState = pid.UpdateTick(m_deltaTime, pidState, currentValue, targetValue);
        Vector3 _torque = m_deltaTime * pidState.Output * rotateAxisLocal;
        RigidBody.AddRelativeTorque(_torque, ForceMode.Acceleration);
    }

    protected void applyAngleTorquePID(PID pid, ref PID.PIDState pidState, Vector3 rotateAxisLocal, float currentValue, float targetValue, float minTurnDot = 0.3f)
    {
        pidState = pid.UpdateAngleTick(m_deltaTime, pidState, currentValue, targetValue);
        float _tiltDotMult = (Vector3.Dot(m_upDirection, Vector3.up) - minTurnDot) / (1.0f - minTurnDot);
        Vector3 _torque = m_deltaTime * pidState.Output * _tiltDotMult * rotateAxisLocal;
        RigidBody.AddRelativeTorque(_torque, ForceMode.Acceleration);
    }
}
