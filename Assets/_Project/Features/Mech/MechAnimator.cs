using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAnimator : MonoBehaviour
{
    private MechController m_mech = null;
    private Transform m_animationRoot = null;

    private void Awake()
    {
        TryGetComponent(out m_mech);
    }

    public void Initialize(Transform animationRoot)
    {
        m_animationRoot = animationRoot;
    }
}
