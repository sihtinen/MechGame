using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using BKUnity;
using static UnityEditor.PlayerSettings;

public class MechController : RigidBodyEntity
{
    [Header("Runtime Parameters")]
    [NonEditable] public Vector2 MoveInput = Vector2.zero;
    [NonEditable] public Vector3 LookTargetWorldPos = Vector3.zero;
    [NonEditable] public float TargetRotY = 0;
    [NonEditable] public float ThrustVelocityHorizontal = 0;
    [NonEditable] public float ThrustVelocityVertical = 0;
    [NonEditable] public bool IsGrounded = false;
    [NonEditable] public bool IsBoosting = false;
    [NonEditable] public RaycastHit GroundHit;

    [NonSerialized] public MechPlayerInput PlayerInputComponent = null;

    [Header("Object References")]
    [SerializeField, Expandable] private PID m_rideHeightPID = null;
    [SerializeField, Expandable] private PID m_pitchRotationPID = null;
    [SerializeField, Expandable] private PID m_yawRotationPID = null;
    [SerializeField, Expandable] private PID m_rollRotationPID = null;
    [SerializeField, Expandable] private MechSettings m_settings = null;
    [Space]
    [SerializeField] private MechBuilder m_mechBuilder = null;
    [SerializeField] private HumanoidAvatarBuilder m_avatarBuilder = null;
    public GenericMechAnimator MechAnimator = null;
    [SerializeField] private List<Transform> m_lookTargets = new();

    private PID.PIDState m_rideHeightPIDState = new PID.PIDState();
    private PID.PIDState m_pitchRotationPIDState = new PID.PIDState();
    private PID.PIDState m_yawRotationPIDState = new PID.PIDState();
    private PID.PIDState m_rollRotationPIDState = new PID.PIDState();

    private Transform m_animationTransformRoot = null;
    private Transform m_mechTransformRoot = null;
    private MechSpineRotator m_spineRotator = null;

    private MechLoadout m_loadout = null;
    private List<Collider> m_mechColliders = new List<Collider>();

    protected override void Awake()
    {
        base.Awake();

        IsBoosting = false;

        TryGetComponent(out PlayerInputComponent);

        m_animationTransformRoot = m_mechBuilder.transform;
    }

    public void InitializeGameplay(InitializeSettings settings)
    {
        m_loadout = settings.Loadout;
        m_mechBuilder.LoadoutAsset = m_loadout;

        m_mechTransformRoot = m_mechBuilder.Build();
        m_mechTransformRoot.transform.SetParent(transform, worldPositionStays: false);

        m_avatarBuilder.SetAvatarTarget(m_mechTransformRoot.gameObject.GetComponent<AvatarTarget>());
        m_avatarBuilder.BuildRuntime();

        m_spineRotator = m_animationTransformRoot.GetComponentInChildren<MechSpineRotator>();

        ignoreSelfCollisions();

        PlayerInputComponent.enabled = settings.IsPlayer;

        setupEquipment(settings.IsPlayer);
    }

    private void setupEquipment(bool isPlayer)
    {
        foreach (var _kvp in m_loadout.Dictionary)
        {
            if (_kvp.Value == null)
                continue;

            var _slotTransform = new GameObject($"{ _kvp.Key}_{_kvp.Value.DisplayName}").transform;
            _slotTransform.SetParent(transform, worldPositionStays: false);

            var _setupData = new Equipment.EquipmentRuntimeSetupData
            {
                Mech = this,
                SlotTransform = _slotTransform,
                SlotType = _kvp.Key,
                IsPlayer = isPlayer,
                InputActionRef = isPlayer ? PlayerInputComponent.GetInputActionRef(_kvp.Key) : null,
            };

            _kvp.Value.InitializeGameplay(_setupData);
        }
    }

    private void ignoreSelfCollisions()
    {
        m_mechTransformRoot.GetComponentsInChildren(includeInactive: true, m_mechColliders);

        for (int i = 0; i < m_mechColliders.Count; i++)
        {
            var _coll = m_mechColliders[i];

            for (int ii = 0; ii < m_mechColliders.Count; ii++)
            {
                if (i == ii)
                    continue;

                var _otherColl = m_mechColliders[ii];

                Physics.IgnoreCollision(_coll, _otherColl);
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < m_lookTargets.Count; i++)
            m_lookTargets[i].position = LookTargetWorldPos;

        if (m_spineRotator != null)
            m_spineRotator.SetRotation(-m_bodyRotateAngle);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        var _rot = RigidBody.rotation.eulerAngles;
        _rot.x = 0;
        _rot.z = 0;
        RigidBody.rotation = Quaternion.Euler(_rot);
        RigidBody.ResetInertiaTensor();

        groundCheck();
        updateDrag();
        updateRideHeightPID();
        updateRotationPIDs();
        updateMovementForce();
        updateThrustForces();

        RigidBody.AddForce(m_settings.GravityMultiplier * Physics.gravity, ForceMode.Acceleration);
    }

    private void updateDrag()
    {
        var _totalVel = RigidBody.velocity;

        Vector3 _verticaVel = Vector3.Project(_totalVel, IsGrounded ? GroundHit.normal : Vector3.up);

        Vector3 _horizontalVel_Old = _totalVel - _verticaVel;
        _totalVel -= _horizontalVel_Old;

        Vector3 _horizontalVel_New = _horizontalVel_Old * (1 - m_deltaTime * m_settings.HorizontalDrag_Ground);
        _totalVel += _horizontalVel_New;

        RigidBody.velocity = _totalVel;
    }

    private void updateMovementForce()
    {
        if (MoveInput.sqrMagnitude <= Mathf.Epsilon)
            return;

        Vector3 _movePlaneUp = IsGrounded ? GroundHit.normal : Vector3.up;
        Quaternion _targetLookQuaternion = Quaternion.Euler(0, TargetRotY, 0);
        Vector3 _targetForward = _targetLookQuaternion * Vector3.forward;
        Vector3 _targetRight = _targetLookQuaternion * Vector3.right;
        Vector3 _inputDirWorldSpace = MoveInput.y * _targetForward + MoveInput.x * _targetRight;
        Vector3 _inputDirProjected = Vector3.ProjectOnPlane(_inputDirWorldSpace, _movePlaneUp).normalized;

        float _directionDot = Vector3.Dot(_inputDirProjected, m_forwardDirection);

        Vector3 _moveVectorRaw = m_settings.MaxMovementSpeed
            * m_settings.MovementForwardDirectionMultCurve.Evaluate(_directionDot)
            * _inputDirProjected.normalized;

        RigidBody.AddForce(m_deltaTime * _moveVectorRaw, ForceMode.Acceleration);

        Debug.DrawLine(
            TransformComponent.position,
            TransformComponent.position + _moveVectorRaw,
            Color.Lerp(Color.red, Color.green, Mathf.InverseLerp(-1f, 1f, _directionDot)),
            m_deltaTime);

        //updateMovementTiltTorque(_moveVectorRaw);
    }

    private void updateThrustForces()
    {
        if (Vector3.Dot(m_upDirection, Vector3.up) < 0.5)
            return;

        if (MoveInput.sqrMagnitude > Mathf.Epsilon)
        {
            Vector3 _movePlaneUp = IsGrounded ? GroundHit.normal : Vector3.up;
            Quaternion _targetLookQuaternion = Quaternion.Euler(0, TargetRotY, 0);
            Vector3 _targetForward = _targetLookQuaternion * Vector3.forward;
            Vector3 _targetRight = _targetLookQuaternion * Vector3.right;
            Vector3 _inputDirWorldSpace = MoveInput.y * _targetForward + MoveInput.x * _targetRight;
            Vector3 _inputDirProjected = Vector3.ProjectOnPlane(_inputDirWorldSpace, _movePlaneUp).normalized;

            RigidBody.AddForce(m_deltaTime * ThrustVelocityHorizontal * _inputDirProjected, ForceMode.Acceleration);
        }

        RigidBody.AddForce(m_deltaTime * ThrustVelocityVertical * m_upDirection, ForceMode.Acceleration);
    }

    private void updateMovementTiltTorque(Vector3 moveVector)
    {
        Vector3 _moveVectorInLocalSpace = TransformComponent.InverseTransformVector(moveVector);
        _moveVectorInLocalSpace.x *= m_settings.SidewaysMovementTiltForceMult;
        _moveVectorInLocalSpace.y = 0;
        _moveVectorInLocalSpace.z *= m_settings.ForwardMovementTiltForceMult;

        float _tiltDotMult = (Vector3.Dot(m_upDirection, Vector3.up) - m_settings.MinTiltDot) / (1.0f - m_settings.MinTiltDot);
        Vector3 _torqueMoveVector = _tiltDotMult * _moveVectorInLocalSpace;

        Vector3 _tiltTorque = Quaternion.Euler(0, 90, 0) * _torqueMoveVector;

        RigidBody.AddRelativeTorque(m_deltaTime * _tiltTorque, ForceMode.Acceleration);
    }

    private void updateRotationPIDs()
    {
        var _axisAngles = TransformComponent.GetAxisAngles();

        //applyTorquePID(
        //    pid: m_pitchRotationPID,
        //    pidState: ref m_pitchRotationPIDState,
        //    rotateAxisLocal: Vector3.right,
        //    currentValue: _axisAngles.Pitch,
        //    targetValue: 0f);

        //applyTorquePID(
        //    pid: m_rollRotationPID,
        //    pidState: ref m_rollRotationPIDState,
        //    rotateAxisLocal: Vector3.forward,
        //    currentValue: _axisAngles.Roll,
        //    targetValue: 0f);

        bool _applyRotation = false;
        float _rotationAngle = TargetRotY;

        if (IsBoosting)
        {
            _applyRotation = true;
        }
        else
        {
            var _bodySettings = m_loadout.Dictionary[EquipmentSlotTypes.Body] as BodyEquipment;

            float _forwardAngle = Vector3.SignedAngle(m_forwardDirection, Vector3.forward, Vector3.down);

            float _angleDifference = Quaternion.Angle(
                Quaternion.Euler(new Vector3(0, _forwardAngle, 0)),
                Quaternion.Euler(new Vector3(0, TargetRotY, 0)));

            Vector3 _moveVel = RigidBody.velocity;
            _moveVel.y = 0;

            _applyRotation = _angleDifference > _bodySettings.MaxRotationAngle_Yaw || _moveVel.sqrMagnitude > 1f;
        }

        if (_applyRotation)
        {
            applyAngleTorquePID(
                pid: m_yawRotationPID,
                pidState: ref m_yawRotationPIDState,
                rotateAxisLocal: Vector3.up,
                currentValue: _axisAngles.Yaw,
                targetValue: _rotationAngle % 360);
        }

        updateBodyRotation();
    }

    private void updateBodyRotation()
    {
        var _bodySettings = m_loadout.Dictionary[EquipmentSlotTypes.Body] as BodyEquipment;

        float _currentAngle = (transform.eulerAngles.y + m_bodyRotateAngle) % 360;
        float _targetAngle = TargetRotY % 360;

        float _angleDiff = Mathf.DeltaAngle(_currentAngle, _targetAngle);
        float _targetRotAngle = m_bodyRotateAngle + _angleDiff;

        if (_targetRotAngle > 180)
            _targetRotAngle -= 360;
        else if (_targetRotAngle < -180)
            _targetRotAngle += 360;

        _targetRotAngle = Mathf.Clamp(_targetRotAngle, -_bodySettings.MaxRotationAngle_Yaw, _bodySettings.MaxRotationAngle_Yaw);

        m_bodyRotateAngle = Mathf.SmoothDamp(
            m_bodyRotateAngle,
            _targetRotAngle,
            ref m_bodyRotateVelocity, 
            _bodySettings.RotationSmoothTime_Yaw, 
            _bodySettings.RotationMaxSpeed_Yaw + RigidBody.angularVelocity.y, 
            m_deltaTime);
    }

    private float m_bodyRotateAngle = 0;
    private float m_bodyRotateVelocity = 0;

    private void groundCheck()
    {
        if (Application.isEditor)
        {
            IsGrounded = RotaryHeart.Lib.PhysicsExtension.Physics.SphereCast(
                origin: transform.position + m_settings.GroundCheckHeight * m_upDirection,
                radius: m_settings.GroundCheckRadius,
                direction: -m_upDirection,
                hitInfo: out GroundHit,
                maxDistance: m_settings.GroundCheckDistance,
                layerMask: m_settings.GroundCheckLayers,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore,
                preview: RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both,
                drawDuration: m_deltaTime,
                hitColor: Color.green,
                noHitColor: Color.red,
                drawDepth: false,
                drawType: RotaryHeart.Lib.PhysicsExtension.CastDrawType.Minimal);
        }
        else
        {
            IsGrounded = Physics.SphereCast(
                origin: transform.position + m_settings.GroundCheckHeight * m_upDirection,
                radius: m_settings.GroundCheckRadius,
                direction: -m_upDirection,
                hitInfo: out GroundHit,
                maxDistance: m_settings.GroundCheckDistance,
                layerMask: m_settings.GroundCheckLayers,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore);
        }

        if (IsGrounded && m_animationTransformRoot != null)
        {
            var _pos = m_animationTransformRoot.position;
            _pos.y = GroundHit.point.y;
            m_animationTransformRoot.position = _pos;
        }
    }

    private void updateRideHeightPID()
    {
        if (IsGrounded == false)
        {
            m_rideHeightPIDState.ResetState();
            return;
        }

        float _hitDistanceCorrected = GroundHit.distance + m_settings.GroundCheckRadius;
        m_rideHeightPIDState = m_rideHeightPID.UpdateTick(m_deltaTime, m_rideHeightPIDState, _hitDistanceCorrected, m_settings.GroundRideHeight);
        Vector3 _force = m_deltaTime * Mathf.Max(m_rideHeightPIDState.Output - ThrustVelocityVertical, 0) * Vector3.up;
        RigidBody.AddForce(_force, ForceMode.Acceleration);
    }

    public void SetLookTargetPos(Vector3 pos)
    {
        LookTargetWorldPos = Vector3.Lerp(LookTargetWorldPos, pos, Time.deltaTime * 20);
    }

    public Vector3 GetLookTargetPos()
    {
        return m_lookTargets[0].position;
    }

    [System.Serializable]
    public class InitializeSettings
    {
        public bool IsPlayer;
        public MechLoadout Loadout;
    }
}