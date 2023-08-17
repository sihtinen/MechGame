using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerButtonIconImageComponent : MonoBehaviour
{
    [SerializeField] private InputActionReference m_inputActionRef = null;
    [SerializeField] private GameObject m_disableObjectWhenController = null;

    private Image m_image = null;

    private void Awake()
    {
        TryGetComponent(out m_image);
    }

    private void Start()
    {
        var _eventSystemComponent = UIEventSystemComponent.Instance;
        if (_eventSystemComponent != null)
        {
            _eventSystemComponent.OnActiveInputDeviceChanged += this.onInputDeviceChanged;
            onInputDeviceChanged(_eventSystemComponent.ActiveInputDevice);
        }
        else
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var _eventSystemComponent = UIEventSystemComponent.Instance;
        if (_eventSystemComponent != null)
            _eventSystemComponent.OnActiveInputDeviceChanged -= this.onInputDeviceChanged;
    }

    private void onInputDeviceChanged(InputDeviceTypes deviceType)
    {
        if (deviceType == InputDeviceTypes.KeyboardAndMouse)
        {
            gameObject.SetActiveOptimized(false);

            if (m_disableObjectWhenController != null)
                m_disableObjectWhenController.SetActiveOptimized(true);

            return;
        }

        var _eventSystemComponent = UIEventSystemComponent.Instance;
        var _iconDatabase = _eventSystemComponent.ControllerButtonIconDatabaseAsset;
        var _iconSprite = _iconDatabase.GetIcon(deviceType, m_inputActionRef);

        if (_iconSprite != null)
            m_image.overrideSprite = _iconSprite;

        gameObject.SetActiveOptimized(_iconSprite != null);

        if (m_disableObjectWhenController != null)
            m_disableObjectWhenController.SetActiveOptimized(_iconSprite == null);
    }
}
