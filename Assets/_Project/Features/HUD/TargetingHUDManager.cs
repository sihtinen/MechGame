using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingHUDManager : SingletonBehaviour<TargetingHUDManager>
{
    [SerializeField] private bool m_enableDebug = false;

    private List<MechProjectileRuntime> m_activeProjectileWeapons = new List<MechProjectileRuntime>();

    private void LateUpdate()
    {
        var _mainCamera = MainCameraComponent.Instance.CameraComponent;

        HUDTargetingElementPool.ResetUsedObjects();

        for (int i = 0; i < m_activeProjectileWeapons.Count; i++)
        {
            var _projectile = m_activeProjectileWeapons[i];
            var _settings = _projectile.Settings;

            for (int ii = 0; ii < _projectile.ValidTargets.Count; ii++)
            {
                var _validTarget = _projectile.ValidTargets[ii];
                var _targetTransform = _validTarget.TransformComponent;

                if (_projectile.ActiveTarget != null && _targetTransform.GetInstanceID() == _projectile.ActiveTarget.TransformComponent.GetInstanceID())
                    continue;

                var _element = generateElement(_targetTransform.position, _mainCamera, 
                    _settings.HUD_ValidTarget, 
                    _settings.HUD_ValidTarget_Size);

                _element.BindToTargetingOption(_validTarget, m_enableDebug);
            }

            if (_projectile.ActiveTarget != null)
            {
                var _element = generateElement(_projectile.ActiveTarget.TransformComponent.position, _mainCamera,
                    _settings.HUD_ActiveTarget,
                    _settings.HUD_ActiveTarget_Size);

                _element.BindToTargetingOption(_projectile.ActiveTarget, m_enableDebug);

                generateElement(_projectile.PredictionPos, _mainCamera,
                    _settings.HUD_Prediction,
                    _settings.HUD_Prediction_Size);
            }
        }
    }

    private HUDTargetingElement generateElement(Vector3 worldPosition, Camera mainCamera, Sprite icon, float iconSize)
    {
        var _anchorPos = mainCamera.WorldToViewportPoint(worldPosition);

        var _hudElement = HUDTargetingElementPool.Get();
        _hudElement.ImageComponent.overrideSprite = icon;
        _hudElement.ImageComponent.enabled = true;
        _hudElement.RectTransformComponent.SetWidth(iconSize);
        _hudElement.RectTransformComponent.SetHeight(iconSize);
        _hudElement.RectTransformComponent.anchorMin = _anchorPos;
        _hudElement.RectTransformComponent.anchorMax = _anchorPos;
        _hudElement.RectTransformComponent.anchoredPosition = Vector2.zero;

        return _hudElement;
    }

    public void RegisterProjectileRuntime(MechProjectileRuntime runtimeComponent)
    {
        if (m_activeProjectileWeapons.Contains(runtimeComponent) == false)
            m_activeProjectileWeapons.Add(runtimeComponent);
    }

    public void UnregisterProjectileRuntime(MechProjectileRuntime runtimeComponent)
    {
        if (m_activeProjectileWeapons.Contains(runtimeComponent))
            m_activeProjectileWeapons.Remove(runtimeComponent);
    }
}