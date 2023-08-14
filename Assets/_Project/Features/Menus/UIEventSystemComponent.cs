using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIEventSystemComponent : SingletonBehaviour<UIEventSystemComponent>
{
    [Header("Runtime Parameters")]
    [NonEditable] public InputDeviceTypes ActiveInputDevice = InputDeviceTypes.KeyboardAndMouse;

    public event Action<InputDeviceTypes> OnActiveInputDeviceChanged = null;

    private InputSystemUIInputModule m_inputModule = null;
    private InputActionMap m_uiActionMap = null;
    public InputActionMap UIActionMap => m_uiActionMap;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_inputModule);

        m_uiActionMap = m_inputModule.actionsAsset.FindActionMap("UI");

        foreach (var _action in m_uiActionMap)
            _action.performed += this.onInputAction;
    }

    private void onInputAction(InputAction.CallbackContext context)
    {
        var _newInputDevice = ActiveInputDevice;

        switch (context.action.activeControl.device.name)
        {
            case "Mouse":
            case "Keyboard":
                _newInputDevice = InputDeviceTypes.KeyboardAndMouse;
                break;
            default:
                Debug.Log(context.action.activeControl.device.name);
                break;
        }

        if (ActiveInputDevice != _newInputDevice)
        {
            ActiveInputDevice = _newInputDevice;
            OnActiveInputDeviceChanged?.Invoke(ActiveInputDevice);
        }
    }

    private void OnDestroy()
    {
        if (m_uiActionMap != null)
        {
            foreach (var _action in m_uiActionMap)
                _action.performed -= this.onInputAction;

            m_uiActionMap = null;
        }
    }
}
