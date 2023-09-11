using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingArea : SingletonBehaviour<TargetingArea>
{
    private RectTransform m_rectTransform = null;
    private MechController m_target = null;

    protected override void Awake()
    {
        base.Awake();

        m_rectTransform = transform as RectTransform;
    }

    public void BindToTarget(MechController mech)
    {
        m_target = mech;
    }

    public bool IsPointInsideArea(Vector3 worldPoint)
    {
        Vector2 _screenPoint = MainCameraComponent.Instance.CameraComponent.WorldToScreenPoint(worldPoint);
        return m_rectTransform.rect.Contains(m_rectTransform.InverseTransformPoint(_screenPoint));
    }

    private void LateUpdate()
    {
        if (m_target == null)
            return;

        m_rectTransform.sizeDelta = m_target.TargetingComponent.Settings.AreaSize;
    }
}
