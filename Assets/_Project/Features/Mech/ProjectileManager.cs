using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ProjectileManager : SingletonBehaviour<ProjectileManager>
{
    [NonSerialized] public int ActiveProjectilesCount = 0;
    public event Action<RaycastHit> OnProjectileHit = null;

    [Header("Debug Settings")]
    [SerializeField] private bool m_drawDebugLines = false;

    private JobHandle? m_jobHandle;
    private QueryParameters m_sphereCastQueryParams;

    private Stack<ProjectileData> m_newProjectiles = new Stack<ProjectileData>();
    private NativeList<ProjectileData> m_projectileData = new NativeList<ProjectileData>();
    private NativeArray<SpherecastCommand> m_sphereCastCommands = new NativeArray<SpherecastCommand>();
    private NativeArray<RaycastHit> m_raycastHits = new NativeArray<RaycastHit>();

    private readonly int DATA_CAPACITY = 2048;
    private readonly int MAX_SPHERECAST_HITS = 4;

    protected override void Awake()
    {
        base.Awake();

        m_projectileData = new NativeList<ProjectileData>(DATA_CAPACITY, Allocator.Persistent);

        m_sphereCastQueryParams = new QueryParameters
        {
            hitBackfaces = false,
            hitMultipleFaces = false,
            hitTriggers = QueryTriggerInteraction.Ignore,
            layerMask = Physics.DefaultRaycastLayers,
        };
    }

    private void OnDestroy()
    {
        if (m_jobHandle.HasValue)
            m_jobHandle.Value.Complete();

        if (m_projectileData.IsCreated)
            m_projectileData.Dispose();

        if (m_sphereCastCommands.IsCreated)
            m_sphereCastCommands.Dispose();

        if (m_raycastHits.IsCreated)
            m_raycastHits.Dispose();
    }

    private void Update()
    {
        if (m_jobHandle.HasValue)
        {
            m_jobHandle.Value.Complete();
            m_jobHandle = null;
            m_sphereCastCommands.Dispose();

            analyzeSphereCastResults();

            m_raycastHits.Dispose();
        }

        while (m_newProjectiles.Count > 0)
        {
            var _newProjectile = m_newProjectiles.Pop();
            m_projectileData.Add(_newProjectile);
        }

        if (m_projectileData.Length > 0)
        {
            var _dataJob = new ProjectileJob()
            {
                Projectiles = m_projectileData,
                DeltaTime = Time.deltaTime,
            };

            m_jobHandle = _dataJob.Schedule(m_projectileData.Length, 16);
        }
    }

    private void analyzeSphereCastResults()
    {
        for (int i = m_projectileData.Length; i-- > 0;)
        {
            var _projectile = m_projectileData[i];

            bool _hitFound = false;

            for (int ii = 0; ii < MAX_SPHERECAST_HITS; ii++)
            {
                var _hit = m_raycastHits[i * MAX_SPHERECAST_HITS + ii];
                int _hitColliderID = _hit.colliderInstanceID;

                if (_hitColliderID <= 0)
                    break;

                var _hitRootTransform = _hit.transform.root;
                int _hitRootInstanceID = _hitRootTransform.GetInstanceID();

                if (_hitRootInstanceID == _projectile.OwnerID)
                    continue;

                if (_hitRootTransform.TryGetComponent(out IDamageable _damageable))
                    _damageable.DealDamage(_projectile.Damage);

                _hitFound = true;
                OnProjectileHit?.Invoke(_hit);

                if (m_drawDebugLines && Application.isEditor)
                    Debug.DrawLine(_projectile.PreviousPosition, _hit.point, Color.red, 0.2f);

                break;
            }

            if (_hitFound)
                m_projectileData.RemoveAt(i);
            else if (m_drawDebugLines && Application.isEditor)
                Debug.DrawLine(_projectile.PreviousPosition, _projectile.Position, Color.yellow, 0.2f);
        }
    }

    private void LateUpdate()
    {
        if (m_jobHandle.HasValue)
        {
            m_jobHandle.Value.Complete();
            m_jobHandle = null;
        }

        for (int i = m_projectileData.Length; i --> 0;)
        {
            var _projectile = m_projectileData[i];

            if (_projectile.IsAlive == false)
                m_projectileData.RemoveAt(i);
        }

        var _machineGunProjectiles = MachineGunProjectiles.Instance;
        if (_machineGunProjectiles != null)
            _machineGunProjectiles.UpdateParticles(m_projectileData);

        ActiveProjectilesCount = m_projectileData.Length;

        if (ActiveProjectilesCount > 0)
        {
            m_sphereCastCommands = new NativeArray<SpherecastCommand>(ActiveProjectilesCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            int _raycastHitCount = ActiveProjectilesCount * MAX_SPHERECAST_HITS;
            m_raycastHits = new NativeArray<RaycastHit>(_raycastHitCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < _raycastHitCount; i++)
                m_raycastHits[i] = new RaycastHit();

            for (int i = 0; i < ActiveProjectilesCount; i++)
            {
                var _projectile = m_projectileData[i];

                m_sphereCastCommands[i] = new SpherecastCommand 
                {
                    origin = _projectile.PreviousPosition,
                    direction = _projectile.Direction,
                    radius = _projectile.CollisionRadius,
                    distance = _projectile.DistanceTraveledThisFrame,
                    queryParameters = m_sphereCastQueryParams
                };
            }

            m_jobHandle = SpherecastCommand.ScheduleBatch(
                commands: m_sphereCastCommands,
                results: m_raycastHits,
                minCommandsPerJob: 1,
                maxHits: MAX_SPHERECAST_HITS);
        }
    }

    public void RegisterNewProjectile(ProjectileData projectile)
    {
        m_newProjectiles.Push(projectile);
    }

    [BurstCompile]
    public struct ProjectileJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeList<ProjectileData> Projectiles;
        [ReadOnly] public float DeltaTime;

        public void Execute(int index)
        {
            var _projectile = Projectiles[index];

            _projectile.AliveTime += DeltaTime;

            if (_projectile.AliveTime > _projectile.Lifetime)
            {
                _projectile.IsAlive = false;
                Projectiles[index] = _projectile;
                return;
            }

            _projectile.PreviousPosition = _projectile.Position;
            _projectile.Position += DeltaTime * _projectile.Speed * _projectile.Direction;
            _projectile.DistanceTraveledThisFrame = math.distance(_projectile.Position, _projectile.PreviousPosition);

            Projectiles[index] = _projectile;
        }
    }
}

public struct ProjectileData
{
    public bool IsAlive;
    public int OwnerID;
    public int Damage;
    public float CollisionRadius;
    public float AliveTime;
    public float Lifetime;
    public float Speed;
    public float DistanceTraveledThisFrame;
    public float3 PreviousPosition;
    public float3 Position;
    public float3 Direction;
}