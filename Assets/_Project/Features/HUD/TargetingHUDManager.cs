using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingHUDManager : SingletonBehaviour<TargetingHUDManager>
{
    [Header("HUD Settings")]
    public Sprite HUDTex_ValidTarget = null;
    public Sprite HUDTex_LockOnTarget = null;
    public Sprite HUDTex_Prediction = null;
    [Space]
    public float HUDTex_ValidTarget_Size;
    public float HUDTex_LockOnTarget_Size;
    public float HUDTex_Prediction_Size;

    [Header("Debug Settings")]
    [SerializeField] private bool m_enableDebug = false;

    private MechTargeting m_target = null;

    private void LateUpdate()
    {
        if (m_target == null)
            return;

        var _mainCamera = MainCameraComponent.Instance.CameraComponent;

        HUDTargetingElementPool.ResetUsedObjects();

        for (int ii = 0; ii < m_target.ValidTargets.Count; ii++)
        {
            var _validTarget = m_target.ValidTargets[ii];
            var _targetTransform = _validTarget.ContextTargetComponent.TransformComponent;

            if (m_target.ActiveTarget != null && _targetTransform.GetInstanceID() == m_target.ActiveTarget.ContextTargetComponent.TransformComponent.GetInstanceID())
                continue;

            var _element = generateElement(_targetTransform.position, _mainCamera,
                HUDTex_ValidTarget,
                HUDTex_ValidTarget_Size);

            _element.BindToTargetingOption(_validTarget, m_enableDebug);
            _element.gameObject.SetActiveOptimized(true);
        }

        if (m_target.ActiveTarget == null)
            return;

        var _lockOnElement = generateElement(m_target.ActiveTarget.GetPosition(), _mainCamera,
            HUDTex_LockOnTarget,
            HUDTex_LockOnTarget_Size);

        _lockOnElement.BindToTargetingOption(m_target.ActiveTarget, m_enableDebug);
        _lockOnElement.gameObject.SetActiveOptimized(true);

        for (int i = 0; i < m_target.PredictionPositions.Count; i++)
        {
            var _element = generateElement(m_target.PredictionPositions[i], _mainCamera,
                HUDTex_Prediction,
                HUDTex_Prediction_Size);

            _element.gameObject.SetActiveOptimized(true);
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

    public void BindToComponent(MechTargeting target)
    {
        m_target = target;
    }
}