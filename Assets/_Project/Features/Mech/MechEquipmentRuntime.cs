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

    public InputDeviceTypes GetActiveInputDeviceType()
    {
        if (m_inputActionRef == null || m_inputActionRef.action.activeControl == null)
            return InputDeviceTypes.Inactive;

        switch (m_inputActionRef.action.activeControl.device.displayName)
        {
            case "Xbox Controller":
                return InputDeviceTypes.Xbox;

            case "DualSense Wireless Controller":
                return InputDeviceTypes.PlayStation;

            default:
                return InputDeviceTypes.KeyboardAndMouse;
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
}
