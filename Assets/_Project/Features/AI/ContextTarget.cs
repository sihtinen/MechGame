using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextTarget : MonoBehaviour
{
    public ContextTargetLayers TargetLayers = ContextTargetLayers.None;

    [NonSerialized] public Transform TransformComponent = null;
    [NonSerialized] public Rigidbody RigidbodyComponent = null;

    [NonSerialized] public Vector3 PositionPreviousFrame = Vector3.zero;
    [NonSerialized] public Vector3 TransformVelocity = Vector3.zero;
    [NonSerialized] public Vector3 CalculatedAcceleration = Vector3.zero;

    private List<Collider> m_myColliders = new List<Collider>();

    private void Awake()
    {
        TransformComponent = transform;
        TryGetComponent(out RigidbodyComponent);
    }

    private void Start()
    {
        PositionPreviousFrame = TransformComponent.position;
        GetComponentsInChildren(includeInactive: true, m_myColliders);
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            ObjectCollection<ContextTarget>.RegisterObject(this);
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            ObjectCollection<ContextTarget>.UnregisterObject(this);
    }

    public Vector3 GetVelocity()
    {
        if (RigidbodyComponent != null && RigidbodyComponent.isKinematic == false)
            return RigidbodyComponent.velocity;

        return TransformVelocity;
    }

    public bool HasColliderWithID(int id)
    {
        for (int i = 0; i < m_myColliders.Count; i++)
        {
            if (m_myColliders[i].GetInstanceID() == id)
                return true;
        }

        return false;
    }
}
