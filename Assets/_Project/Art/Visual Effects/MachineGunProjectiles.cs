using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class MachineGunProjectiles : SingletonBehaviour<MachineGunProjectiles>
{
    [SerializeField] private string m_bufferPropertyName = "ProjectileDataBuffer";
    [SerializeField] private int m_bufferCapacity = 4096;

    private VisualEffect m_visualEffect = null;
    private GraphicsBuffer m_buffer = null;
    private List<ProjectileRenderData> m_renderDataList = new List<ProjectileRenderData>();

    protected override void Awake()
    {
        base.Awake();

        int _stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(ProjectileRenderData));
        m_buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_bufferCapacity, _stride);

        m_renderDataList = new List<ProjectileRenderData>(capacity: m_bufferCapacity);
        for (int i = 0; i < m_bufferCapacity; i++)
        {
            m_renderDataList.Add(new ProjectileRenderData
            {
                Size = 0,
            });
        }

        TryGetComponent(out m_visualEffect);
        m_visualEffect.SetGraphicsBuffer(m_bufferPropertyName, m_buffer);
    }

    private void OnDestroy()
    {
        if (m_buffer != null)
            m_buffer.Release();
    }

    public void UpdateParticles(NativeList<ProjectileData> projectileDataList)
    {
        int _aliveProjectilesCount = projectileDataList.Length;

        for (int i = 0; i < m_bufferCapacity; i++)
        {
            var _renderData = m_renderDataList[i];

            if (i < _aliveProjectilesCount)
            {
                var _projectile = projectileDataList[i];
                _renderData.Position = _projectile.Position;
                _renderData.Velocity = _projectile.Speed * _projectile.Direction;
                _renderData.Size = 1.5f;
            }
            else
                _renderData.Size = 0;

            m_renderDataList[i] = _renderData;
        }

        m_buffer.SetData(m_renderDataList);
    }

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct ProjectileRenderData
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Size;
    }
}
