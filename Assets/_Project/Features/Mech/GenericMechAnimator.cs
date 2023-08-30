using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMechAnimator : MonoBehaviour
{
    [SerializeField] private float m_movementSpeedMultiplier = 1.0f;
    [SerializeField] private MechController m_mech = null;

    private Animator m_animator = null;
    private string m_animState = null;

    private void Awake()
    {
        TryGetComponent(out m_animator);
    }

    private void Update()
    {
        if (m_mech == null)
            return;

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
}
