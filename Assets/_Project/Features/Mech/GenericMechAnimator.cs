using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMechAnimator : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float m_leanAmount = 0.08f;
    [SerializeField] private float m_movementSpeedMultiplier = 1.0f;

    [Header("Object References")]
    [SerializeField] private MechController m_mech = null;

    private Animator m_animator = null;
    private string m_animState = null;
    private List<MechIKSource> m_ikSources = new();

    private void Awake()
    {
        TryGetComponent(out m_animator);
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
            playState("Movement Boost");
        }
        else
        {
            if (_velMagnitude < _minVelMag)
                playState("Idle");
            else
                playState("Movement Bipedal");
        }
    }

    private void playState(string stateName)
    {
        if (m_animState == stateName)
            return;

        m_animState = stateName;
        m_animator.CrossFadeInFixedTime(m_animState, 0.2f);
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
}
