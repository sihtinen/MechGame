using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraComponent : SingletonBehaviour<MainCameraComponent>
{
    [NonSerialized] public Transform TransformComponent = null;
    [NonSerialized] public Camera CameraComponent = null;

    protected override void Awake()
    {
        base.Awake();

        TransformComponent = transform;
        TryGetComponent(out CameraComponent);
    }
}
