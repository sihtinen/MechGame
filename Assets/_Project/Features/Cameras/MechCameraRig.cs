using Cinemachine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class MechCameraRig : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float m_mouseSensitivity = 1f;
    [SerializeField] private float m_stickSensitivityPitch = 1f;
    [SerializeField] private float m_stickSensitivityYaw = 1f;
    [SerializeField] private float m_minPitch = -80f;
    [SerializeField] private float m_maxPitch = 80f;
    [SerializeField] private float m_additionalFOVPerVelocity = 0.03f;
    [Space]
    [SerializeField] private MechCameraRigPose m_normalPose = null;
    [SerializeField] private MechCameraRigPose m_dashBoostPose = null;

    [Header("Object References")]
    [SerializeField] private CinemachineVirtualCamera m_vcam = null;
    [SerializeField] private CinemachineCameraOffset m_vcamOffset = null;
    [SerializeField] private InputActionReference m_lookAction = null;
    [SerializeField] private MechController m_mechController = null;

    private float m_targetRotationPitch = 20f;
    private float m_targetRotationYaw = 0f;

    private float m_heightOffset;
    private float m_followDistance;
    private float m_fov;

    private Vector3 m_velocityOffsetVel = Vector3.zero;

    private void Awake()
    {
        if (m_lookAction != null)
            m_lookAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_fov = m_normalPose.FieldOfView;
        m_followDistance = m_normalPose.FollowDistance;
        m_heightOffset = m_normalPose.FollowHeightOffset;
    }

    private void OnDestroy()
    {
        if (m_lookAction != null)
            m_lookAction.action.Disable();
    }

    private void Update()
    {
        updateTargetInput();
        updatePosition();
    }

    private void updateTargetInput()
    {
        if (m_lookAction == null || m_lookAction.action.activeControl == null)
            return;

        var _lookInput = m_lookAction.action.ReadValue<Vector2>();

        if (m_lookAction.action.activeControl.device.displayName == "Mouse")
        {
            m_targetRotationPitch -= _lookInput.y * m_mouseSensitivity;
            m_targetRotationYaw += _lookInput.x * m_mouseSensitivity;
        }
        else
        {
            m_targetRotationPitch -= Time.deltaTime * _lookInput.y * m_stickSensitivityPitch;
            m_targetRotationYaw += Time.deltaTime * _lookInput.x * m_stickSensitivityYaw;
        }

        if (m_targetRotationYaw < 0f)
            m_targetRotationYaw += 360f;
        else if (m_targetRotationYaw >= 360f)
            m_targetRotationYaw -= 360f;

        m_targetRotationPitch = Mathf.Clamp(m_targetRotationPitch, m_minPitch, m_maxPitch);

        if (m_mechController != null)
            m_mechController.TargetRotY = m_targetRotationYaw;
    }

    private void updatePosition()
    {
        if (m_mechController == null)
            return;

        var _pose = m_normalPose;
        var _velocity = m_mechController.RigidBody.velocity;
        var _velocityNormalized = _velocity.normalized;

        if (m_mechController.IsDashBoosting)
            _pose = m_dashBoostPose;

        float _targetHeightOffset = _pose.FollowHeightOffset;
        m_heightOffset = Mathf.MoveTowards(m_heightOffset, _targetHeightOffset, Time.deltaTime * _pose.FollowHeightOffsetUpdateSpeed);

        float _targetFollowDistance = _pose.FollowDistance;
        m_followDistance = Mathf.MoveTowards(m_followDistance, _targetFollowDistance, Time.deltaTime * _pose.FollowDistanceUpdateSpeed);

        transform.position = m_mechController.transform.position + m_heightOffset * Vector3.up;
        transform.eulerAngles = new Vector3(m_targetRotationPitch, m_targetRotationYaw, 0f);
        transform.position -= m_followDistance * transform.forward;

        float _targetFOV = _pose.FieldOfView;
        _targetFOV += _velocity.magnitude * m_additionalFOVPerVelocity;
        m_fov = Mathf.MoveTowards(m_fov, _targetFOV, Time.deltaTime * _pose.FieldOfViewUpdateSpeed);
        m_vcam.m_Lens.FieldOfView = m_fov;

        Vector3 _upVel = Vector3.Project(_velocity, transform.up);
        Vector3 _rightVel = Vector3.Project(_velocity, transform.right);
        Vector3 _forwardVel = Vector3.Project(_velocity, transform.forward);

        float _upVelDot = Vector3.Dot(_velocityNormalized, transform.up);
        float _rightVelDot = Vector3.Dot(_velocityNormalized, transform.right);
        float _forwardVelDot = Vector3.Dot(_velocityNormalized, transform.forward);

        const float OFFSET_MAGIC_MULT = -0.05f;

        var _targetOffset = new Vector3
        {
            x = _rightVelDot * _rightVel.magnitude * _pose.VelocityOffsetFactor.x * OFFSET_MAGIC_MULT,
            y = _upVelDot * _upVel.magnitude * _pose.VelocityOffsetFactor.y * OFFSET_MAGIC_MULT,
            z = _forwardVelDot * _forwardVel.magnitude * _pose.VelocityOffsetFactor.z * OFFSET_MAGIC_MULT,
        };

        _targetOffset.x = Mathf.Clamp(_targetOffset.x, _pose.VelocityOffsetMin.x, _pose.VelocityOffsetMax.x);
        _targetOffset.y = Mathf.Clamp(_targetOffset.y, _pose.VelocityOffsetMin.y, _pose.VelocityOffsetMax.y);
        _targetOffset.z = Mathf.Clamp(_targetOffset.z, _pose.VelocityOffsetMin.z, _pose.VelocityOffsetMax.z);

        m_vcamOffset.m_Offset = Vector3.SmoothDamp(
            m_vcamOffset.m_Offset,
            _targetOffset, 
            ref m_velocityOffsetVel, 
            _pose.VelocityOffsetSmoothTime, 
            _pose.VelocityOffsetMaxSpeed);
    }
}
