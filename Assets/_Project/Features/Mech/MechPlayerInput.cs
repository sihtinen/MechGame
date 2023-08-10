using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechPlayerInput : SingletonBehaviour<MechPlayerInput>
{
    [SerializeField] private InputActionReference m_moveInputRef = null;
    public InputActionReference LeftShoulderInputRef = null;
    public InputActionReference LeftArmInputRef = null;
    public InputActionReference RightShoulderInputRef = null;
    public InputActionReference RightArmInputRef = null;

    private MechController m_mechController = null;

    private void OnEnable()
    {
        if (Application.isPlaying == false)
            return;

        TryGetComponent(out m_mechController);

        if (m_moveInputRef != null)
            m_moveInputRef.action.Enable();

        if (LeftShoulderInputRef != null)
            LeftShoulderInputRef.action.Enable();

        if (LeftArmInputRef != null)
            LeftArmInputRef.action.Enable();

        if (RightShoulderInputRef != null)
            RightShoulderInputRef.action.Enable();

        if (RightArmInputRef != null)
            RightArmInputRef.action.Enable();
    }

    private void OnDisable()
    {
        if (Application.isPlaying == false)
            return;

        if (m_moveInputRef != null)
            m_moveInputRef.action.Disable();

        if (LeftShoulderInputRef != null)
            LeftShoulderInputRef.action.Disable();

        if (LeftArmInputRef != null)
            LeftArmInputRef.action.Disable();

        if (RightShoulderInputRef != null)
            RightShoulderInputRef.action.Disable();

        if (RightArmInputRef != null)
            RightArmInputRef.action.Disable();
    }

    private void Update()
    {
        if (m_moveInputRef == null)
            return;

        m_mechController.MoveInput = m_moveInputRef.action.ReadValue<Vector2>();
    }
}
