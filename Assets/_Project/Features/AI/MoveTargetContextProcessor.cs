using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/AI/Context Processors/New Move Target Context Processor")]
public class MoveTargetContextProcessor : ContextProcessor
{
    [SerializeField] private AnimationCurve m_dotFallOffCurve = new AnimationCurve();

    public override void Process(ContextEntity entity)
    {
        int _moveTargetCount = entity.MoveTargets.Count;

        for (int i = 0; i < _moveTargetCount; i++) 
        { 
            var _moveTarget = entity.MoveTargets[i];
            float3 _toMoveTarget = (_moveTarget.position - entity.TransformComponent.position);
            float3 _toMoveTargetNormalized = math.normalize(_toMoveTarget);

            for (int ii = 0; ii < entity.SamplerCount; ii++)
            {
                var _sampler = entity.Data.Samplers[ii];

                float _samplerDirectionDot = math.dot(_toMoveTargetNormalized, _sampler.Direction);

                float _effect = m_dotFallOffCurve.Evaluate(_samplerDirectionDot);

                calculateValue(ref _sampler.Value, _effect);

                entity.Data.Samplers[ii] = _sampler;
            }
        }
    }
}
