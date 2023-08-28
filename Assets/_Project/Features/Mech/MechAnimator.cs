using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAnimator : MonoBehaviour
{
    [Header("Animation Parameters")]
    [NonEditable] public float LegRoot_Right_Rotation_Forward = 0;
    [NonEditable] public float LegRoot_Left_Rotation_Forward = 0;
    [NonEditable] public float Leg01_Right_Rotation_Forward = 0;
    [NonEditable] public float Leg01_Left_Rotation_Forward = 0;

    [Header("Animated Bones")]
    [SerializeField] private AnimatedBone m_legRootR = new ();
    [SerializeField] private AnimatedBone m_legRootL = new();

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

        m_legRootR.Initialize(m_mechVisualsRoot);
        m_legRootL.Initialize(m_mechVisualsRoot);
    }

    private void Update()
    {
        return;

        m_legRootR.UpdateRotation(new Vector3(0, 0, LegRoot_Right_Rotation_Forward * 35));
        m_legRootL.UpdateRotation(new Vector3(0, 0, LegRoot_Left_Rotation_Forward * -35));

        float _height = 0.5f * Mathf.Abs(LegRoot_Left_Rotation_Forward - Leg01_Left_Rotation_Forward);
        m_mechVisualsRoot.transform.localPosition += new Vector3(0, _height, 0);

        var _horizontalVel = m_mech.RigidBody.velocity;
        _horizontalVel.y = 0;

        m_animator.SetFloat("MovementSpeed", _horizontalVel.magnitude * 0.03f);
    }

    [Serializable]
    public class AnimatedBone
    {
        public string BoneName;

        private Vector3 m_defaultLocalEuler;
        private Transform m_bone = null;

        public void Initialize(Transform root)
        {
            m_bone = root.FindChildRecursive(BoneName);
            m_defaultLocalEuler = m_bone.localEulerAngles;
        }

        public void UpdateRotation(Vector3 localEulerRotation)
        {
            m_bone.localEulerAngles = m_defaultLocalEuler + localEulerRotation;
        }
    }
}
