using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIScreen<T> : SingletonBehaviour<T> where T : MonoBehaviour
{
    [NonSerialized] public bool IsOpened = false;

    [Header("UI Screen General Settings")]
    [SerializeField] protected GameObject m_gamepadFirstActiveElement = null;

    protected Canvas m_canvas;

    private int m_openedFrame;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_canvas);
    }

    protected virtual void Start()
    {
        var _inputEventComponent = UIEventSystemComponent.Instance;
        if (_inputEventComponent != null)
            _inputEventComponent.OnActiveInputDeviceChanged += this.onInputDeviceChanged;

        this.Close();
    }

    protected virtual void OnDestroy()
    {
        var _inputEventComponent = UIEventSystemComponent.Instance;
        if (_inputEventComponent != null)
            _inputEventComponent.OnActiveInputDeviceChanged -= this.onInputDeviceChanged;
    }

    protected virtual void onInputDeviceChanged(InputDeviceTypes deviceType)
    {
        if (IsOpened == false)
            return;

        switch (deviceType)
        {
            case InputDeviceTypes.KeyboardAndMouse:

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                break;

            default:

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                if (m_gamepadFirstActiveElement != null)
                    EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, m_gamepadFirstActiveElement);

                break;
        }
    }

    public void Open()
    {
        IsOpened = true;
        m_canvas.enabled = true;
        m_openedFrame = Time.frameCount;

        var _inputEventComponent = UIEventSystemComponent.Instance;
        if (_inputEventComponent != null && _inputEventComponent.ActiveInputDevice != InputDeviceTypes.KeyboardAndMouse)
        {
            EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, m_gamepadFirstActiveElement);
        }

        onOpened();

        gameObject.SetActiveOptimized(true);
    }

    public void Close()
    {
        gameObject.SetActiveOptimized(false);

        IsOpened = false;
        m_canvas.enabled = false;

        onClosed();
    }

    protected bool wasOpenedThisFrame() => m_openedFrame == Time.frameCount;

    protected virtual void onOpened() { }
    protected virtual void onClosed() { }
}