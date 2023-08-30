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
    private Transform m_animationRoot = null;

    private List<MechIKSource> m_ikSources = new();

    private void Awake()
    {
        TryGetComponent(out m_animator);
        TryGetComponent(out m_mech);
    }

    public void Initialize(Transform animationRoot)
    {
        m_animationRoot = animationRoot;
        animationRoot.GetComponentsInChildren(includeInactive: true, m_ikSources);
    }

    private void Update()
    {
        if (m_animationRoot == null)
            return;

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

        m_animationRoot.localEulerAngles = Vector3.zero;

        var _horizontalVel = m_mech.RigidBody.velocity;
        _horizontalVel.y = 0;

        float _forwardAmount = Vector3.Dot(_horizontalVel.normalized, m_animationRoot.forward);
        float _rightAmount = Vector3.Dot(_horizontalVel.normalized, m_animationRoot.right);

        m_animationRoot.Rotate(new Vector3(
            _forwardAmount * m_leanAmount * _horizontalVel.magnitude, 
            0,
            -_rightAmount * m_leanAmount * _horizontalVel.magnitude), Space.Self);
    }
}
