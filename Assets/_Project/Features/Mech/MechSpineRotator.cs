using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MechSpineRotator : MonoBehaviour
{
    [SerializeField] private Vector3 m_eulerOffset = Vector3.zero;

    private MultiAimConstraint m_aimConstraint = null;

    private void Awake()
    {
        TryGetComponent(out m_aimConstraint);
    }

    public void SetRotation(float rotation)
    {
        m_aimConstraint.data.offset = m_eulerOffset + new Vector3(0, 0, rotation);
    }

    public Transform GetSpineTransform()
    {
        return m_aimConstraint.data.constrainedObject;
    }
}
