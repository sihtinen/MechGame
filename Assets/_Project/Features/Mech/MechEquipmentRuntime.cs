using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MechEquipmentRuntime : MonoBehaviour
{
    protected InputActionReference m_inputActionRef = null;

    public abstract void InitializeGameplay(MechController mech, Equipment settings);

    public void BindToInput(InputActionReference inputRef)
    {
        m_inputActionRef = inputRef;
        m_inputActionRef.action.started += this.onInputStarted;
        m_inputActionRef.action.canceled += this.onInputCanceled;
    }

    public InputDeviceType GetActiveInputDeviceType()
    {
        if (m_inputActionRef == null || m_inputActionRef.action.activeControl == null)
            return InputDeviceType.Inactive;

        switch (m_inputActionRef.action.activeControl.device.displayName)
        {
            case "Xbox Controller":
                return InputDeviceType.Xbox;

            case "DualSense Wireless Controller":
                return InputDeviceType.Playstation;

            default:
                return InputDeviceType.Keyboard;
        }
    }

    private void OnDestroy()
    {
        if (m_inputActionRef != null)
        {
            m_inputActionRef.action.started -= this.onInputStarted;
            m_inputActionRef.action.canceled -= this.onInputCanceled;
        }
    }

    protected virtual void onInputStarted(InputAction.CallbackContext context) { }
    protected virtual void onInputCanceled(InputAction.CallbackContext context) { }

    public enum InputDeviceType
    {
        Inactive = -1,
        Keyboard = 0,
        Xbox = 1,
        Playstation = 2,
    }
}
