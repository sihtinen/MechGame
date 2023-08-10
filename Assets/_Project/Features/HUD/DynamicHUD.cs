using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DynamicHUD : MonoBehaviour
{
    [SerializeField] private float m_distance = 100f;
    [SerializeField, Range(0f, 1f)] private float m_targetHorizontalSmoothingAmount = 0.2f;
    [SerializeField, Range(0f, 1f)] private float m_cameraDirectionAmount = 0.2f;
    [SerializeField] private float m_tiltVelocityRotationAmount = 0;
    [SerializeField] private float m_cameraPitchAmount = 0f;
    [SerializeField] private float m_cameraOffscreenAngleRotation;
    [SerializeField] private float m_cameraUpOffset = 0f;
    [SerializeField] private float m_minRotationSqrVelocity = 1f;
    [SerializeField] private Transform m_target = null;

    private Vector3 m_defaultAnchorMin;
    private Vector3 m_defaultAnchorMax;
    private Vector3 m_defaultAnchorPos;

    private Camera m_mainCam = null;
    private RectTransform m_rectTransform = null;
    private IDynamicHUDTarget m_targetInterface = null;

    private void Start()
    {
        m_mainCam = Camera.main;
        m_rectTransform = transform as RectTransform;

        m_defaultAnchorMin = m_rectTransform.anchorMin;
        m_defaultAnchorMax = m_rectTransform.anchorMax;
        m_defaultAnchorPos = m_rectTransform.anchoredPosition;

        if (m_target != null)
            m_target.TryGetComponent(out m_targetInterface);
    }

    private void LateUpdate()
    {
        if (m_target == null)
            return;

        Vector3 _finalForward = m_targetInterface != null ? m_targetInterface.GetForwardDirection() : m_target.forward;

        Vector3 _targetHorizontalForward = _finalForward;
        _targetHorizontalForward.y = 0;
        _targetHorizontalForward.Normalize();

        _finalForward = Vector3.Lerp(_finalForward, _targetHorizontalForward, m_targetHorizontalSmoothingAmount);
        _finalForward = Vector3.Lerp(_finalForward, m_mainCam.transform.forward, m_cameraDirectionAmount);

        Quaternion _cRot = m_mainCam.transform.rotation;
        float _cameraPitch = Mathf.Rad2Deg * Mathf.Atan2(2 * _cRot.x * _cRot.w - 2 * _cRot.y * _cRot.z, 1 - 2 * _cRot.x * _cRot.x - 2 * _cRot.z * _cRot.z);

        Vector3 _samplePos = m_mainCam.transform.position
            + m_distance * _finalForward
            + m_cameraPitchAmount * -_cameraPitch * m_mainCam.transform.up
            + m_cameraUpOffset * m_mainCam.transform.up;

        var _hudPos = m_mainCam.WorldToViewportPoint(_samplePos);

        Vector3 _offset = _hudPos - new Vector3(0.5f, 0.5f, 0f);

        m_rectTransform.anchorMin = m_defaultAnchorMin + _offset;
        m_rectTransform.anchorMax = m_defaultAnchorMax + _offset;
        m_rectTransform.anchoredPosition = m_defaultAnchorPos;

        float _rotationX = 0;
        float _rotationY = 0;
        float _rotationZ = 0;

        Vector3 _toSamplePosFromCamera = _samplePos - m_mainCam.transform.position;
        _rotationY = m_cameraOffscreenAngleRotation * Vector3.Dot(m_mainCam.transform.right, _toSamplePosFromCamera.normalized);

        if (m_targetInterface != null)
        {
            Vector3 _vel = m_targetInterface.GetVelocity();
            Vector3 _projectedVel = Vector3.Project(_vel, m_mainCam.transform.right);

            if (_projectedVel.sqrMagnitude > m_minRotationSqrVelocity)
            {
                int _rotationDir = Vector3.Dot(_projectedVel.normalized, m_mainCam.transform.right) >= 0 ? 1 : -1;
                _rotationZ = _rotationDir * (_projectedVel.magnitude - m_minRotationSqrVelocity) * m_tiltVelocityRotationAmount;
            }
        }

        m_rectTransform.eulerAngles = new Vector3(_rotationX, _rotationY, _rotationZ);
    }

    public interface IDynamicHUDTarget
    {
        public Vector3 GetForwardDirection();
        public Vector3 GetVelocity();
    }
}
