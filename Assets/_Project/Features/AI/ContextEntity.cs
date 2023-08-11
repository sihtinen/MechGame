using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ContextEntity : MonoBehaviour
{
    [NonEditable] public float Result_Value = 0;
    [NonEditable] public Vector3 Result_Offset = Vector3.zero;

    [NonSerialized] public bool JobUpdateEnabled = true;
    [NonSerialized] public int SamplerCount = 0;
    [NonSerialized] public ContextData Data;
    [NonSerialized] public Transform TransformComponent = null;
    [NonSerialized] public List<int> SamplersBestOrder = new List<int>();

    public List<Transform> MoveTargets = new List<Transform>();

    [Header("Debug Settings")]
    [SerializeField] private bool m_drawGizmos = false;

    [Header("Object References")]
    [SerializeField, Expandable] private ContextProcessorSettings m_settings = null;
    [SerializeField, Expandable] private List<ContextProcessor> m_processors = new List<ContextProcessor>();

    private NativeArray<SpherecastCommand> m_sphereCastCommands;
    private NativeArray<RaycastHit> m_hitResults;
    private JobHandle? m_raycastJob;

    private bool m_didPhysicsUpdate = false;
    private static readonly int RAYCAST_MAX_HITS = 4;

    private List<Collider> m_ownColliders = new List<Collider>();

    private void Start()
    {
        TransformComponent = transform;
        TransformComponent.GetComponentsInChildren(includeInactive: true, m_ownColliders);

        Data = new ContextData();
        SamplerCount = 0;

        for (int i = 0; i < m_settings.SampleLayers.Count; i++)
            SamplerCount += m_settings.SampleLayers[i].SampleCount;

        for (int i = 0; i < SamplerCount; i++)
            SamplersBestOrder.Add(i);

        Data.Samplers = new NativeArray<ContextSampler>(SamplerCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        m_sphereCastCommands = new NativeArray<SpherecastCommand>(SamplerCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        m_hitResults = new NativeArray<RaycastHit>(SamplerCount * RAYCAST_MAX_HITS, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        int _cellIndex = 0;

        for (int i = 0; i < m_settings.SampleLayers.Count; i++)
        {
            var _layer = m_settings.SampleLayers[i];

            for (int ii = 0; ii < _layer.SampleCount; ii++)
            {
                var _sampler = new ContextSampler();
                _sampler.Value = 0f;

                float radians = 2 * Mathf.PI / _layer.SampleCount * ii;
                float vertical = Mathf.Sin(radians);
                float horizontal = Mathf.Cos(radians);
                _sampler.PositionOffset = new float3(_layer.CollisionDistance * horizontal, _layer.HeightOffset, _layer.CollisionDistance * vertical);
                _sampler.Direction = math.normalize(_sampler.PositionOffset);
                _sampler.Distance = math.length(_sampler.PositionOffset);

                Data.Samplers[_cellIndex] = _sampler;
                _cellIndex++;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_raycastJob.HasValue && m_raycastJob.Value.IsCompleted == false)
            m_raycastJob.Value.Complete();

        if (Data.IsCreated)
            Data.Dispose();

        if (m_sphereCastCommands.IsCreated)
            m_sphereCastCommands.Dispose();

        if (m_hitResults.IsCreated)
            m_hitResults.Dispose();
    }

    private void FixedUpdate()
    {
        if (m_raycastJob.HasValue)
        {
            m_raycastJob.Value.Complete();

            if (JobUpdateEnabled)
            {
                initializeSamplers();
                evaluateProcessors();
                calculateResults();
            }
        }

        m_didPhysicsUpdate = true;
    }

    private void calculateResults()
    {
        SamplersBestOrder.Sort((a, b) =>
        {
            if (Data.Samplers[a].Value > Data.Samplers[b].Value)
                return -1;
            else
                return 1;
        });

        float _targetResultValue = 0;
        Vector3 _targetResultOffset = Vector3.zero;

        for (int i = 0; i < m_settings.ResultInterpolationSteps; i++)
        {
            var _samplerIndex = SamplersBestOrder[i];
            var _sampler = Data.Samplers[_samplerIndex];

            float _lerpAmount = 1.0f / (i + 1);

            _targetResultValue = Mathf.Lerp(_targetResultValue, _sampler.Value, _lerpAmount);
            _targetResultOffset = Vector3.Lerp(_targetResultOffset, _sampler.PositionOffset, _lerpAmount);
        }

        Result_Value = Mathf.SmoothDamp(Result_Value, _targetResultValue, ref m_resultValueVelocity, m_settings.ResultSmoothTime, float.MaxValue, deltaTime: Time.fixedDeltaTime);
        Result_Offset = Vector3.SmoothDamp(Result_Offset, _targetResultOffset, ref m_resultOffsetVelocity, m_settings.ResultSmoothTime, float.MaxValue, deltaTime: Time.fixedDeltaTime);
    }

    private float m_resultValueVelocity = 0;
    private Vector3 m_resultOffsetVelocity = Vector3.zero;

    private void evaluateProcessors()
    {
        for (int i = 0; i < m_processors.Count; i++)
        {
            var _processor = m_processors[i];
            _processor.Process(this);
        }
    }

    private void initializeSamplers()
    {
        for (int i = 0; i < SamplerCount; i++)
        {
            var _sampler = Data.Samplers[i];
            var _raycastCommand = m_sphereCastCommands[i];
            bool _hitFound = false;

            for (int ii = 0; ii < RAYCAST_MAX_HITS; ii++)
            {
                var _hit = m_hitResults[i * RAYCAST_MAX_HITS + ii];
                int _hitColliderID = _hit.colliderInstanceID;

                if (_hitColliderID <= 0)
                    break;

                if (isSelf(_hitColliderID) == false)
                {
                    _hitFound = true;
                    _sampler.Value = -1.0f + _hit.distance / _raycastCommand.distance;
                    break;
                }
            }

            if (_hitFound == false)
                _sampler.Value = 0f;

            Data.Samplers[i] = _sampler;
        }
    }

    private bool isSelf(int colliderID)
    {
        for (int i = 0; i < m_ownColliders.Count; i++)
        {
            if (m_ownColliders[i].GetInstanceID() == colliderID)
                return true;
        }

        return false;
    }

    private void LateUpdate()
    {
        if (m_didPhysicsUpdate == false)
            return;

        m_didPhysicsUpdate = false;

        if (JobUpdateEnabled)
            scheduleRaycastJob();
    }

    private void scheduleRaycastJob()
    {
        float3 _fromPos = TransformComponent.position;
        var _layerMask = Physics.DefaultRaycastLayers;

        for (int i = 0; i < SamplerCount; i++)
        {
            var _sampler = Data.Samplers[i];

            m_sphereCastCommands[i] = new SpherecastCommand(
                origin: _fromPos,
                radius: m_settings.CollisionSphereRadius,
                direction: _sampler.Direction,
                distance: _sampler.Distance,
                queryParameters: new QueryParameters
                {
                    layerMask = _layerMask,
                    hitMultipleFaces = true,
                    hitBackfaces = false,
                    hitTriggers = QueryTriggerInteraction.Ignore,
                });
        }

        m_raycastJob = SpherecastCommand.ScheduleBatch(
            m_sphereCastCommands,
            m_hitResults,
            1,
            maxHits: RAYCAST_MAX_HITS);
    }

    private void OnDrawGizmos()
    {
        if (m_drawGizmos == false || Data.IsCreated == false)
            return;

        for (int i = 0; i < Data.Samplers.Length; i++)
        {
            var _sampler = Data.Samplers[i];

            float _samplerValAbs = Mathf.Abs(_sampler.Value);

            Gizmos.color = _sampler.Value < 0f ?
                Color.Lerp(Color.white, Color.red, _samplerValAbs) :
                Color.Lerp(Color.white, Color.green, _samplerValAbs);

            Gizmos.DrawLine(TransformComponent.position, TransformComponent.position + _samplerValAbs * _sampler.Distance * (Vector3)_sampler.Direction);
        }
    }
}
