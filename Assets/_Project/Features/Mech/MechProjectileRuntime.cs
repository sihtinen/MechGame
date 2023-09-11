using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class MechProjectileRuntime : MechEquipmentRuntime
{
    [System.NonSerialized] public int RemainingUses;
    [System.NonSerialized] public Vector3 PredictionPos = Vector3.zero;

    private bool m_firedLastFrame = false;
    private int m_rootTransformID;
    private float m_previousUseTime;
    private EquipmentSlotTypes m_slotType;

    private Transform m_transform = null;
    private MechController m_mechController = null;
    private ProjectileEquipment m_settings = null;
    private WeaponVisualsManager m_weaponVisualsManager = null;

    public ProjectileEquipment Settings => m_settings;

    private void Awake()
    {
        m_transform = transform;
        m_previousUseTime = Time.time;
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
        if (m_mechController == null)
            return;

        var _activeTarget = m_mechController.TargetingComponent.ActiveTarget;

        if (_activeTarget != null && _activeTarget.TransformComponent.root.TryGetComponent(out Rigidbody _rb))
        {
            calculatePredictionPos(_activeTarget.TransformComponent.position, _rb.velocity);
            m_mechController.TargetingComponent.PredictionPositions.Add(PredictionPos);
        }
    }

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

    private Vector3 getShootDirection(Vector3 sourcePos)
    {
        if (m_mechController.TargetingComponent.ActiveTarget != null)
        {
            if (TargetingArea.Instance.IsPointInsideArea(m_mechController.TargetingComponent.ActiveTarget.TransformComponent.position))
                return (PredictionPos - sourcePos).normalized;
        }

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
}
