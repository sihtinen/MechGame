using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextTarget : MonoBehaviour
{
    public ContextTargetLayers TargetLayers = ContextTargetLayers.None;

    [NonSerialized] public Transform TransformComponent = null;
    [NonSerialized] public Rigidbody RigidbodyComponent = null;

    private void Awake()
    {
        TransformComponent = transform;
        TryGetComponent(out RigidbodyComponent);
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
}
