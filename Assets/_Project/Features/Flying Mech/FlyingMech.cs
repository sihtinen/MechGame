using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FlyingMech : RigidBodyEntity, IDamageable
{
    [NonEditable] public int CurrentHealth;
    [NonEditable] public Vector3 MoveTargetPos;

    [Header("Object References")]
    [SerializeField, Expandable] private FlyingMechSettings m_settings = null;
    [SerializeField, Expandable] private PID m_pidFlyHeight = null;
    [SerializeField, Expandable] private PID m_pidForwardThrust = null;
    [SerializeField, Expandable] private PID m_pidSidewaysThrust = null;
    [SerializeField, Expandable] private PID m_pidPitch = null;
    [SerializeField, Expandable] private PID m_pidRoll = null;
    [SerializeField, Expandable] private PID m_pidYaw = null;
    [Space]
    [SerializeField] private Transform m_lookTarget = null;

    private PID.PIDState m_psFlyHeight = new PID.PIDState();
    private PID.PIDState m_psForwardThrust = new PID.PIDState();
    private PID.PIDState m_psSidewaysThrust = new PID.PIDState();
    private PID.PIDState m_psRotationPitch = new PID.PIDState();
    private PID.PIDState m_psRotationRoll = new PID.PIDState();
    private PID.PIDState m_psRotationYaw = new PID.PIDState();

    private ContextEntity m_contextEntity = null;

    public event Action<IDamageable.HealthUpdateParams> OnHealthUpdated = null;

    protected override void Awake()
    {
        base.Awake();

        CurrentHealth = m_settings.Health;

        MoveTargetPos = TransformComponent.position;
        TryGetComponent(out m_contextEntity);
    }

    private void Start()
    {
        var _healthBar = HealthBarPool.Get();
        _healthBar.BindToInterface(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (CurrentHealth > 0)
        {
            MoveTargetPos =
                m_contextEntity.TransformComponent.position +
                m_contextEntity.Result_Value * m_contextEntity.Result_Offset;

            updateFlyHeightPID();
            updateRotationPIDs();
            updateMovementPIDs();
        }

        RigidBody.AddForce(m_settings.GravityMultiplier * Physics.gravity, ForceMode.Acceleration);
    }

    private void updateRotationPIDs()
    {
        var _axisAngles = TransformComponent.GetAxisAngles();

        applyTorquePID(
            pid: m_pidPitch,
            pidState: ref m_psRotationPitch,
            rotateAxisLocal: Vector3.right,
            currentValue: _axisAngles.Pitch,
            targetValue: 0);

        applyTorquePID(
            pid: m_pidRoll,
            pidState: ref m_psRotationRoll,
            rotateAxisLocal: Vector3.forward,
            currentValue: _axisAngles.Roll,
            targetValue: 0);

        Vector3 _lookTargetPos = m_lookTarget.position;

        if (Physics.Linecast(TransformComponent.position, m_lookTarget.position, out var _hitInfo))
        {
            if (_hitInfo.collider.transform.root.GetInstanceID() != m_lookTarget.GetInstanceID())
                _lookTargetPos = MoveTargetPos;
        }

        Vector3 _toLookTarget = _lookTargetPos - TransformComponent.position;
        var _toLookTargetEuler = Quaternion.LookRotation(_toLookTarget.normalized, Vector3.up).eulerAngles;

        applyAngleTorquePID(
            pid: m_pidYaw,
            pidState: ref m_psRotationYaw,
            rotateAxisLocal: Vector3.up,
            currentValue: TransformComponent.eulerAngles.y,
            targetValue: _toLookTargetEuler.y);
    }


    private void updateFlyHeightPID()
    {
        float _targetHeight = MoveTargetPos.y;

        _targetHeight += m_settings.FlyHeightNoiseStrength * Mathf.PerlinNoise(
            m_time * m_settings.FlyHeightNoiseSpeed.x, 
            m_time * m_settings.FlyHeightNoiseSpeed.y);

        m_psFlyHeight = m_pidFlyHeight.UpdateTick(m_deltaTime, m_psFlyHeight, TransformComponent.position.y, _targetHeight);
        Vector3 _force = m_deltaTime * m_psFlyHeight.Output * TransformComponent.up;
        RigidBody.AddForce(_force, ForceMode.Acceleration);
    }

    private void updateMovementPIDs()
    {
        Vector3 _toTarget = MoveTargetPos - TransformComponent.position;
        Vector3 _toTargetNormalized = _toTarget.normalized;
        Vector3 _relativeVelocity = RigidBody.GetRelativePointVelocity(RigidBody.position);

        if (Application.isEditor && m_settings.DrawDebugLines)
            Debug.DrawLine(TransformComponent.position, TransformComponent.position + _toTarget, m_settings.DebugColor_ToMoveTarget, m_deltaTime);

        float _forwardDot = Vector3.Dot(m_forwardDirection, _toTargetNormalized);
        Vector3 _toTargetVelocity = Vector3.Project(RigidBody.velocity, _toTargetNormalized);

        float _rightDot = Vector3.Dot(m_rightDirection, _toTargetNormalized);

        applyForcePID(
            pid: m_pidForwardThrust,
            pidState: ref m_psForwardThrust,
            forceDirectionLocal: Vector3.forward,
            currentValue: _forwardDot >= 0 ? _toTargetVelocity.magnitude : -_toTargetVelocity.magnitude,
            targetValue: (1.0f - Mathf.Abs(_rightDot)) * m_settings.ForwardMoveSpeed);

        applyForcePID(
            pid: m_pidSidewaysThrust,
            pidState: ref m_psSidewaysThrust,
            forceDirectionLocal: -Vector3.right,
            currentValue: _toTarget.magnitude + Mathf.Abs(_relativeVelocity.x),
            targetValue: 0);
    }

    int IDamageable.GetCurrentHealth() => CurrentHealth;
    int IDamageable.GetMaxHealth() => m_settings.Health;
    Transform IDamageable.GetTransform() => TransformComponent;

    void IDamageable.DealDamage(int damage)
    {
        if (CurrentHealth <= 0)
            return;

        int _prevHealth = CurrentHealth;
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            m_contextEntity.enabled = false;
        }

        OnHealthUpdated?.Invoke(new IDamageable.HealthUpdateParams
        {
            PreviousHealth = _prevHealth,
            CurrentHealth = CurrentHealth,
        });
    }
}