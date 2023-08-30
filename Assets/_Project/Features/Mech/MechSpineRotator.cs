using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MechSpineRotator : MonoBehaviour
{
    private MultiAimConstraint m_aimConstraint = null;

    private void Awake()
    {
        TryGetComponent(out m_aimConstraint);
    }

    public void SetRotation(float rotation)
    {
        m_aimConstraint.data.offset = new Vector3(0, 0, rotation);
    }
}
