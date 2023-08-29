using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAnimator : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float m_leanAmount = 1f;
    
    private Animator m_animator = null;
    private MechController m_mech = null;
    private Transform m_mechVisualsRoot = null;

    private void Awake()
    {
        TryGetComponent(out m_animator);
        TryGetComponent(out m_mech);
    }

    public void Initialize(Transform mechVisuals)
    {
        m_mechVisualsRoot = mechVisuals;
    }

    private void Update()
    {
        if (m_mechVisualsRoot == null)
            return;

        m_mechVisualsRoot.localEulerAngles = Vector3.zero;

        var _horizontalVel = m_mech.RigidBody.velocity;
        _horizontalVel.y = 0;

        float _forwardAmount = Vector3.Dot(_horizontalVel.normalized, m_mechVisualsRoot.forward);
        float _rightAmount = Vector3.Dot(_horizontalVel.normalized, m_mechVisualsRoot.right);

        m_mechVisualsRoot.Rotate(new Vector3(
            _forwardAmount * m_leanAmount * _horizontalVel.magnitude, 
            0,
            -_rightAmount * m_leanAmount * _horizontalVel.magnitude), Space.Self);
    }
}
