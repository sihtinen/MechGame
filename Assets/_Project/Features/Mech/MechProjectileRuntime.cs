using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class MechProjectileRuntime : MechEquipmentRuntime
{
    [System.NonSerialized] public int RemainingUses;
    [System.NonSerialized] public List<TargetingOption> ValidTargets = new List<TargetingOption>();
    [System.NonSerialized] public TargetingOption ActiveTarget = null;
    [System.NonSerialized] public Vector3 PredictionPos = Vector3.zero;

    private int m_rootTransformID;
    private float m_previousUseTime;
    private RaycastHit m_hitInfo;
    private EquipmentSlotTypes m_slotType;

    private Transform m_transform = null;
    private MechController m_mechController = null;
    private ProjectileEquipment m_settings = null;
    private WeaponVisualsManager m_weaponVisualsManager = null;

    public ProjectileEquipment Settings => m_settings;
    private Stack<TargetingOption> m_targetingOptionPool = new Stack<TargetingOption>();
    private Stack<TargetingOption> m_usedTargetingOptions = new Stack<TargetingOption>();

    private void Awake()
    {
        m_transform = transform;

        m_previousUseTime = Time.time;

        for (int i = 0; i < 32; i++)
            m_targetingOptionPool.Push(new TargetingOption());
    }

    private void Start()
    {
        m_rootTransformID = m_transform.root.GetInstanceID();
    }

    public override void InitializeGameplay(MechController mech, Equipment settings, EquipmentSlotTypes slotType)
    {
        m_mechController = mech;
        m_settings = settings as ProjectileEquipment;
        m_slotType = slotType;

        RemainingUses = m_settings.UseCount;

        switch (m_slotType)
        {
            case EquipmentSlotTypes.LeftArm:
                m_mechController.MechAnimator.IsWieldingWeapon_Left = true;
                break;

            case EquipmentSlotTypes.RightArm:
                m_mechController.MechAnimator.IsWieldingWeapon_Right = true;
                break;
        }

        if (m_mechController.MechAnimator.EquipmentVisuals.TryGetValue(slotType, out Transform _weaponRoot))
            _weaponRoot.TryGetComponent(out m_weaponVisualsManager);
    }

    private void FixedUpdate()
    {
        ActiveTarget = null;
        ValidTargets.Clear();

        while (m_usedTargetingOptions.Count > 0)
        {
            var _option = m_usedTargetingOptions.Pop();
            m_targetingOptionPool.Push(_option);
        }    

        var _mainCam = MainCameraComponent.Instance;
        var _possibleTargets = ContextUtils.GetActiveTargets(m_settings.TargetLayers);

        for (int i = 0; i < _possibleTargets.Count; i++)
        {
            var _possibleTarget = _possibleTargets[i];

            if (_possibleTarget.TryGetComponent(out IDamageable _damageable) && _damageable.GetCurrentHealth() <= 0)
                continue;

            Vector3 _toTarget = _possibleTarget.TransformComponent.position - m_transform.position;
            Vector3 _toTargetNormalized = _toTarget.normalized;

            if (Vector3.Dot(_toTargetNormalized, _mainCam.TransformComponent.forward) < 0f)
                continue;

            if (Vector3.Dot(_toTargetNormalized, m_transform.forward) < m_settings.TargetingMinDot)
                continue;

            if (_toTarget.magnitude > m_settings.TargetingDistance)
                continue;

            if (Physics.Linecast(
                _mainCam.TransformComponent.position, 
                _possibleTarget.TransformComponent.position, 
                out m_hitInfo, 
                Physics.DefaultRaycastLayers))
            {
                if (m_hitInfo.collider.transform.root.GetInstanceID() != _possibleTarget.TransformComponent.GetInstanceID())
                    continue;
            }

            var _newOption = getTargetingOption();
            _newOption.TransformComponent = _possibleTarget.TransformComponent;

            ValidTargets.Add(_newOption);
        }

        float _bestDot = float.MinValue;
        Vector3 _cameraForward = _mainCam.TransformComponent.forward;

        for (int i = 0; i < ValidTargets.Count; i++)
        {
            var _target = ValidTargets[i];

            Vector3 _fromCameraToTarget = _target.TransformComponent.position - _mainCam.TransformComponent.position;
            float _cameraDot = Vector3.Dot(_fromCameraToTarget.normalized, _cameraForward);
            _target.CameraDotScore = _cameraDot * m_settings.TargetingCameraDotScore;

            Vector3 _fromProjectileToTarget = _target.TransformComponent.position - m_transform.position;
            float _projectileDot = Vector3.Dot(_fromProjectileToTarget.normalized, m_transform.forward);
            _projectileDot = (_projectileDot - m_settings.TargetingMinDot) / (1.0f - m_settings.TargetingMinDot);
            _target.ComponentDotScore = _projectileDot * m_settings.TargetingComponentDotScore;

            float _distFromOptimalDistance = Mathf.Max(Mathf.InverseLerp(
                m_settings.TargetingOptimalDistance, 
                m_settings.TargetingDistance, 
                _fromProjectileToTarget.magnitude), 0);

            _target.DistScore = (1.0f - _distFromOptimalDistance) * m_settings.TargetingDistanceScoreMultiplier;

            _target.TotalScore = _target.CameraDotScore 
                + _target.ComponentDotScore 
                + _target.DistScore;

            if (_target.TotalScore > _bestDot)
            {
                _bestDot = _target.TotalScore;
                ActiveTarget = _target;
                PredictionPos = _target.TransformComponent.position;
            }
        }

        if (ActiveTarget != null && ActiveTarget.TransformComponent.root.TryGetComponent(out Rigidbody _rb))
            calculatePredictionPos(ActiveTarget.TransformComponent.position, _rb.velocity);
    }

    private bool m_firedLastFrame = false;

    private void Update()
    {
        bool _firedLastFrame = m_firedLastFrame;
        m_firedLastFrame = false;

        if (RemainingUses <= 0 || m_inputActionRef == null)
            return;

        if (m_inputActionRef.action.IsPressed() == false)
            return;

        float _timeNow = Time.time;
        float _timeBetweenUses = _timeNow - m_previousUseTime;
        float _betweenShotsWaitTime = 1.0f / m_settings.UseRatePerSecond;

        if (_timeBetweenUses < _betweenShotsWaitTime)
            return;

        int _newProjectilesCount = _firedLastFrame ? (int)Mathf.Floor(1 + Time.deltaTime / _betweenShotsWaitTime) : 1;

        m_firedLastFrame = true;
        m_previousUseTime = _timeNow;

        for (int i = 0; i < _newProjectilesCount; i++)
        {
            RemainingUses--;

            Vector3 _sourcePos = m_weaponVisualsManager != null ? m_weaponVisualsManager.GetWeaponBarrelPosition() : m_transform.position;
            Vector3 _direction = getShootDirection(_sourcePos);

            if (i > 0)
                _sourcePos += Time.deltaTime * _direction * m_settings.Velocity;

            ProjectileManager.Instance.RegisterNewProjectile(new ProjectileData
            {
                IsAlive = true,
                OwnerID = m_rootTransformID,
                Damage = m_settings.Damage,
                CollisionRadius = m_settings.CollisionRadius,
                AliveTime = 0,
                Lifetime = m_settings.Lifetime,
                Speed = m_settings.Velocity,
                DistanceTraveledThisFrame = 0,
                Direction = _direction,
                Position = _sourcePos,
                PreviousPosition = _sourcePos,
            });
        }

        m_mechController.MechAnimator.WeaponFired(m_slotType);

        m_weaponVisualsManager?.TriggerFireEffects();
    }

    private Vector3 getShootDirection(Vector3 _sourcePos)
    {
        if (ActiveTarget != null)
            return (PredictionPos - _sourcePos).normalized;
        else
            return m_weaponVisualsManager.GetWeaponBarrelForward();
    }

    private void calculatePredictionPos(Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 _toTarget = targetPosition - m_transform.position;

        //Ignoring Y for now. Add gravity compensation later, for more simple formula and clean game design around it
        //_toTarget.y = 0;
        //targetVelocity.y = 0;

        //solving quadratic ecuation from t*t(Vx*Vx + Vy*Vy - S*S) + 2*t*(Vx*Qx)(Vy*Qy) + Qx*Qx + Qy*Qy = 0
        float a = Vector3.Dot(targetVelocity, targetVelocity) - (m_settings.Velocity * m_settings.Velocity); //Dot is basicly (targetSpeed.x * targetSpeed.x) + (targetSpeed.y * targetSpeed.y)
        float b = 2 * Vector3.Dot(targetVelocity, _toTarget); //Dot is basicly (targetSpeed.x * q.x) + (targetSpeed.y * q.y)
        float c = Vector3.Dot(_toTarget, _toTarget); //Dot is basicly (q.x * q.x) + (q.y * q.y)

        //Discriminant
        float D = Mathf.Sqrt((b * b) - 4 * a * c);

        float t1 = (-b + D) / (2 * a);
        float t2 = (-b - D) / (2 * a);

        float time = Mathf.Max(t1, t2);
        Vector3 ret = targetPosition + targetVelocity * time;

        PredictionPos = ret;
    }

    private TargetingOption getTargetingOption()
    {
        if (m_targetingOptionPool.Count > 0)
            return m_targetingOptionPool.Pop();

        return new TargetingOption();
    }

    [System.Serializable]
    public class TargetingOption
    {
        public Transform TransformComponent = null;
        public float CameraDotScore;
        public float ComponentDotScore;
        public float DistScore;
        public float TotalScore;
    }
}
