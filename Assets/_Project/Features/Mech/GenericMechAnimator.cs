using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMechAnimator : MonoBehaviour
{
    [Header("Runtime Parameters")]
    [NonEditable] public bool IsWieldingWeapon_Left = false;
    [NonEditable] public bool IsWieldingWeapon_Right = false;
    [NonEditable] public float AimAmount_Left = 0;
    [NonEditable] public float AimAmount_Right = 0;

    [Header("Visual Settings")]
    [SerializeField] private float m_leanAmount = 0.08f;
    [SerializeField] private float m_movementSpeedMultiplier = 1.0f;

    [Header("Object References")]
    [SerializeField] private MechController m_mech = null;
    [SerializeField] private Transform m_spineBone = null;
    [SerializeField] private Transform m_lookTarget = null;

    private Animator m_animator = null;
    private List<MechIKSource> m_ikSources = new();

    private string[] m_animStates = new string[3];

    private void Awake()
    {
        TryGetComponent(out m_animator);

        m_animStates[0] = string.Empty;
        m_animStates[1] = string.Empty;
        m_animStates[2] = string.Empty;
    }

    private void Start()
    {
        transform.GetComponentsInChildren(includeInactive: true, m_ikSources);
    }

    private void LateUpdate()
    {
        if (m_mech == null)
            return;

        updateMovement();
        updateBodyLeanRotation();

        m_animator.SetLayerWeight(1, IsWieldingWeapon_Left ? 1 : 0);
        m_animator.SetLayerWeight(2, IsWieldingWeapon_Right ? 1 : 0);

        m_animator.SetFloat("LeftArm_AimAmount", AimAmount_Left);
        m_animator.SetFloat("RightArm_AimAmount", AimAmount_Right);

        Vector3 _toLookTarget = (m_lookTarget.position - m_spineBone.position).normalized;

        float _aimUpDot = Vector3.Dot(-m_spineBone.forward, _toLookTarget);
        float _aimRightDot = Vector3.Dot(-m_spineBone.right, _toLookTarget);

        smoothSetAnimatorFloat("RightArm_DotUp", _aimUpDot);
        smoothSetAnimatorFloat("LeftArm_DotUp", _aimUpDot);
        smoothSetAnimatorFloat("RightArm_DotRight", _aimRightDot);
        smoothSetAnimatorFloat("LeftArm_DotRight", _aimRightDot);

        if (AimAmount_Left > 0f)
            AimAmount_Left -= Time.deltaTime * 0.25f;

        if (AimAmount_Right > 0f)
            AimAmount_Right -= Time.deltaTime * 0.25f;
    }

    private void smoothSetAnimatorFloat(string paramName, float value, float speed = 6f)
    {
        var _currentValue = m_animator.GetFloat(paramName);
        _currentValue = Mathf.Lerp(_currentValue, value, Time.deltaTime * speed);
        m_animator.SetFloat(paramName, _currentValue);
    }

    private void updateMovement()
    {
        var _horizontalVel = m_mech.RigidBody.velocity;
        _horizontalVel.y = 0;

        float _velMagnitude = _horizontalVel.magnitude;
        float _minVelMag = 2f;

        m_animator.SetFloat("MoveSpeed", _horizontalVel.magnitude * m_movementSpeedMultiplier);

        float _forwardDirectionDot = Vector3.Dot(_horizontalVel.normalized, m_mech.transform.forward);
        float _rightDirectionDot = Vector3.Dot(_horizontalVel.normalized, m_mech.transform.right);

        float _moveMagMult = Mathf.Min((_velMagnitude - _minVelMag) / 50f, 1);

        m_animator.SetFloat("MoveDirZ", _forwardDirectionDot * _moveMagMult);
        m_animator.SetFloat("MoveDirX", _rightDirectionDot * _moveMagMult);

        if (m_mech.IsBoosting)
        {
            playState(0, "Movement Boost");
        }
        else
        {
            if (_velMagnitude < _minVelMag)
                playState(0, "Idle");
            else
                playState(0, "Movement Bipedal");
        }
    }

    private void playState(int layer, string stateName)
    {
        if (m_animStates[layer] == stateName)
            return;

        m_animStates[layer] = stateName;
        m_animator.CrossFadeInFixedTime(m_animStates[layer], 0.2f);
    }

    private void updateBodyLeanRotation()
    {
        Vector3 _lookTargetPos = m_mech.GetLookTargetPos();

        for (int i = 0; i < m_ikSources.Count; i++)
        {
            var _source = m_ikSources[i];

            switch (_source.MyType)
            {
                case MechIKSource.SourceType.LookTarget:
                    _source.transform.position = _lookTargetPos;
                    break;
            }
        }

        transform.localEulerAngles = Vector3.zero;

        var _horizontalVel = m_mech.RigidBody.velocity;
        _horizontalVel.y = 0;

        float _forwardAmount = Vector3.Dot(_horizontalVel.normalized, transform.forward);
        float _rightAmount = Vector3.Dot(_horizontalVel.normalized, transform.right);

        transform.Rotate(new Vector3(
            _forwardAmount * m_leanAmount * _horizontalVel.magnitude,
            0,
            -_rightAmount * m_leanAmount * _horizontalVel.magnitude), Space.Self);
    }

    public void WeaponFired(EquipmentSlotTypes slotType)
    {
        switch (slotType)
        {
            case EquipmentSlotTypes.LeftArm:
                AimAmount_Left = 1f;
                break;

            case EquipmentSlotTypes.RightArm:
                AimAmount_Right = 1f;
                break;
        }
    }
}
