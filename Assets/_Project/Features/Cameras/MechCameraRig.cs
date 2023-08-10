using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class MechCameraRig : MonoBehaviour
{
    [SerializeField] private float m_mouseSensitivity = 1f;
    [SerializeField] private float m_stickSensitivityPitch = 1f;
    [SerializeField] private float m_stickSensitivityYaw = 1f;
    [SerializeField] private float m_followHeightOffset = 12f;
    [SerializeField] private float m_followDistance = 30f;

    [Space]
    [SerializeField] private float m_minPitch = -80f;
    [SerializeField] private float m_maxPitch = 80f;

    [SerializeField] private InputActionReference m_lookAction = null;
    [SerializeField] private MechController m_mechController = null;

    private float m_targetRotationPitch = 20f;
    private float m_targetRotationYaw = 0f;

    private void Awake()
    {
        if (m_lookAction != null)
            m_lookAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        transform.position = m_mechController.transform.position + m_followHeightOffset * Vector3.up;

        transform.eulerAngles = new Vector3(m_targetRotationPitch, m_targetRotationYaw, 0f);

        transform.position -= m_followDistance * transform.forward;
    }
}
