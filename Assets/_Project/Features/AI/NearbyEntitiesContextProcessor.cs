using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/AI/Context Processors/New Nearby Entities Context Processor")]
public class NearbyEntitiesContextProcessor : ContextProcessor
{
    [Header("Nearby Entities Context Settings")]
    [SerializeField, Min(0)] private float m_effectDistance = 200;
    [SerializeField] private AnimationCurve m_effectCurve = new AnimationCurve();
    [SerializeField ,Range(-1f, 1f)] private float m_scoreOverDistanceMultiplier = 1.0f;
    [SerializeField] private ContextTargetLayers m_targetLayers = ContextTargetLayers.None;

    public override void Process(ContextEntity entity)
    {
        int _sourceEntityRootID = entity.TransformComponent.root.GetInstanceID();

        var _targetEntities = ContextUtils.GetActiveTargets(m_targetLayers);
        int _targetEntitiesCount = _targetEntities.Count;

        for (int i = 0; i < _targetEntitiesCount; i++)
        {
            var _targetEntity = _targetEntities[i];

            if (_targetEntity.TransformComponent.root.GetInstanceID() == _sourceEntityRootID)
                continue;

            for (int ii = 0; ii < entity.SamplerCount; ii++)
            {
                var _sampler = entity.Data.Samplers[ii];

                float _distanceToSampler = Vector3.Distance(
                    _targetEntity.TransformComponent.position, 
                    entity.TransformComponent.position + (Vector3)_sampler.PositionOffset);

                if (_distanceToSampler > m_effectDistance)
                    continue;

                float _curveValue = m_effectCurve.Evaluate(1.0f - (_distanceToSampler / m_effectDistance));
                float _effect = _curveValue * m_scoreOverDistanceMultiplier;

                calculateValue(ref _sampler.Value, _effect);

                entity.Data.Samplers[ii] = _sampler;
            }
        }
    }
}
