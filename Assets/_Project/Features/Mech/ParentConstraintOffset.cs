using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintOffset : MonoBehaviour
{
    public Vector3 PosOffset = Vector3.zero;
    public Vector3 EulerOffset = Vector3.zero;

    //private ParentConstraint m_parentConstraint = null;

    //private void Awake()
    //{
    //    TryGetComponent(out m_parentConstraint);
    //}

    //private void Update()
    //{
    //    if (m_parentConstraint == null)
    //        return;

    //    m_parentConstraint.SetTranslationOffset(0, PosOffset);
    //    m_parentConstraint.SetRotationOffset(0, EulerOffset);
    //}
}
