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
    [Space]
    [SerializeField] private float m_positionSmoothTime = 0.1f;
    [SerializeField] private float m_positionMaxSpeed = 10f;
    [Space]
    [SerializeField] private float m_rotationSmoothTime = 0.1f;
    [SerializeField] private float m_rotationMaxSpeed = 10f;

    private Vector3 m_defaultAnchorMin;
    private Vector3 m_defaultAnchorMax;
    private Vector3 m_defaultAnchorPos;
    private Vector3 m_currentOffset = Vector3.zero;
    private Vector3 m_positionVelocity = Vector3.zero;
    private Vector3 m_eulerVelocity = Vector3.zero;

    private Camera m_mainCam = null;
    private RectTransform m_rectTransform = null;

    private void Start()
    {
        m_mainCam = Camera.main;
        m_rectTransform = transform as RectTransform;

        m_defaultAnchorMin = m_rectTransform.anchorMin;
        m_defaultAnchorMax = m_rectTransform.anchorMax;
        m_defaultAnchorPos = m_rectTransform.anchoredPosition;
    }

    private void LateUpdate()
    {
        var _dynamicHUDTarget = DynamicHUDTarget.Instance;

        if (_dynamicHUDTarget == null)
            return;

        Vector3 _facingDir = _dynamicHUDTarget.GetFacingDirection();

        Vector3 _targetHorizontalForward = _facingDir;
        _targetHorizontalForward.y = 0;
        _targetHorizontalForward.Normalize();

        _facingDir = Vector3.Lerp(_facingDir, _targetHorizontalForward, m_targetHorizontalSmoothingAmount);
        _facingDir = Vector3.Lerp(_facingDir, m_mainCam.transform.forward, m_cameraDirectionAmount);

        Quaternion _cRot = m_mainCam.transform.rotation;
        float _cameraPitch = Mathf.Rad2Deg * Mathf.Atan2(2 * _cRot.x * _cRot.w - 2 * _cRot.y * _cRot.z, 1 - 2 * _cRot.x * _cRot.x - 2 * _cRot.z * _cRot.z);

        Vector3 _samplePos = m_mainCam.transform.position
            + m_distance * _facingDir
            + m_cameraPitchAmount * -_cameraPitch * m_mainCam.transform.up
            + m_cameraUpOffset * m_mainCam.transform.up;

        var _hudPos = m_mainCam.WorldToViewportPoint(_samplePos);

        Vector3 _targetOffset = _hudPos - new Vector3(0.5f, 0.5f, 0f);

        m_currentOffset = Vector3.SmoothDamp(
            m_currentOffset, 
            _targetOffset,
            ref m_positionVelocity,
            smoothTime: m_positionSmoothTime,
            maxSpeed: m_positionMaxSpeed);

        m_rectTransform.anchorMin = m_defaultAnchorMin + m_currentOffset;
        m_rectTransform.anchorMax = m_defaultAnchorMax + m_currentOffset;
        m_rectTransform.anchoredPosition = m_defaultAnchorPos;

        float _rotationX = 0;
        float _rotationY = 0;
        float _rotationZ = 0;

        Vector3 _toSamplePosFromCamera = _samplePos - m_mainCam.transform.position;
        _rotationY = m_cameraOffscreenAngleRotation * Vector3.Dot(m_mainCam.transform.right, _toSamplePosFromCamera.normalized);

        Vector3 _vel = _dynamicHUDTarget.GetVelocity();
        Vector3 _projectedVel = Vector3.Project(_vel, m_mainCam.transform.right);

        if (_projectedVel.sqrMagnitude > m_minRotationSqrVelocity)
        {
            int _rotationDir = Vector3.Dot(_projectedVel.normalized, m_mainCam.transform.right) >= 0 ? 1 : -1;
            _rotationZ = _rotationDir * (_projectedVel.magnitude - m_minRotationSqrVelocity) * m_tiltVelocityRotationAmount;
        }

        var _targetEulerAngles = new Vector3(_rotationX % 360, _rotationY % 360, _rotationZ % 360);

        m_rectTransform.rotation = SmoothDampQuaternion(
            m_rectTransform.rotation, 
            Quaternion.Euler( _targetEulerAngles ), 
            ref m_eulerVelocity, 
            smoothTime: m_rotationSmoothTime, 
            maxSpeed: m_rotationMaxSpeed);
    }

    public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime, maxSpeed),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime, maxSpeed),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime, maxSpeed)
        );
    }
}
