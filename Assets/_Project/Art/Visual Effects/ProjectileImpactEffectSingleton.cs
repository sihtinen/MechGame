using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.VFX;

public class ProjectileImpactEffectSingleton : SingletonBehaviour<ProjectileImpactEffectSingleton>
{
    private VisualEffect m_visualEffect = null;
    private VFXEventAttribute m_eventAttribute = null;

    private int m_positionParameterID;
    private int m_directionParameterID;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        TryGetComponent(out m_visualEffect);
        m_eventAttribute = m_visualEffect.CreateVFXEventAttribute();

        m_positionParameterID = Shader.PropertyToID("position");
        m_directionParameterID = Shader.PropertyToID("direction");
    }

    private void Start()
    {
        if (Instance != this)
            return;

        ProjectileManager.Instance.OnProjectileHit += this.onProjectileHit;
    }

    private void OnDestroy()
    {
        var _projectileManager = ProjectileManager.Instance;
        if (_projectileManager != null)
            _projectileManager.OnProjectileHit -= this.onProjectileHit;
    }

    private void onProjectileHit(RaycastHit hit)
    {
        PlayAtPosition(hit.point, hit.normal);
    }

    public void PlayAtPosition(Vector3 position, Vector3 normal)
    {
        m_eventAttribute.SetVector3(m_positionParameterID, position + normal);
        m_eventAttribute.SetVector3(m_directionParameterID, normal);

        m_visualEffect.SendEvent(VisualEffectAsset.PlayEventID, m_eventAttribute);
    }
}