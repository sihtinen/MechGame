using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ContextTargetDataUpdater : SingletonBehaviour<ContextTargetDataUpdater>
{
    [Header("Data Update Settings")]
    [SerializeField] private float m_transformVelocitySharpness = 0.4f;

    private void Update()
    {
        var _targets = ObjectCollection<ContextTarget>.AllObjects;

        for (int i = 0; i < _targets.Count; i++)
        {
            var _target = _targets[i];
            var _currentPos = _target.TransformComponent.position;

            if (_target.RigidbodyComponent == null || _target.RigidbodyComponent.isKinematic)
            {
                var _posDiff = _currentPos - _target.PositionPreviousFrame;
                var _targetVelocity = _posDiff / Time.deltaTime;
                _target.TransformVelocity = Vector3.Lerp(_target.TransformVelocity, _targetVelocity, Time.deltaTime * m_transformVelocitySharpness);
            }

            _target.PositionPreviousFrame = _currentPos;
        }
    }
}
