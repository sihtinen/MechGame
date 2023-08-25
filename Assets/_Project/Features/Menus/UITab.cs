using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class UITab : MonoBehaviour
{
    [Header("Base Runtime Parameters")]
    [NonEditable] public bool IsOpened = false;

    [Header("Base Object References")]
    [SerializeField] protected Button m_connectedButton = null;

    private bool m_isOpened = false;
    private InputAction m_cancelAction = null;

    public virtual void Initialize()
    {
        var _uiEventSystem = UIEventSystemComponent.Instance;
        if (_uiEventSystem != null)
        {
            _uiEventSystem.OnActiveInputDeviceChanged += this.onActiveInputDeviceChanged;
            m_cancelAction = _uiEventSystem.UIActionMap.FindAction("Cancel");
            m_cancelAction.started += this.onCancelInput;
        }
    }

    protected virtual void OnDestroy()
    {
        var _uiEventSystem = UIEventSystemComponent.Instance;
        if (_uiEventSystem != null)
            _uiEventSystem.OnActiveInputDeviceChanged -= this.onActiveInputDeviceChanged;

        if (m_cancelAction != null)
        {
            m_cancelAction.started -= this.onCancelInput;
            m_cancelAction = null;
        }
    }

    protected virtual void onCancelInput(InputAction.CallbackContext context) { }

    public void Open()
    {
        IsOpened = true;
        updateInternal();
        onOpened();
    }

    public void Close()
    {
        IsOpened = false;
        updateInternal();
        onClosed();
    }

    protected virtual void onOpened() { }
    protected virtual void onClosed() { }

    private void updateInternal()
    {
        gameObject.SetActiveOptimized(IsOpened);
        updateConnectedButtonHighlight();
    }

    private void updateConnectedButtonHighlight()
    {
        if (m_connectedButton == null)
            return;

        var _colors = m_connectedButton.colors;
        _colors.normalColor = IsOpened ? new Color(255, 128, 0) : Color.white;
        m_connectedButton.colors = _colors;
    }

    protected void setFirstActiveChildAsSelected(Transform root)
    {
        int _childCount = root.childCount;

        for (int i = 0; i < _childCount; i++)
        {
            var _child = root.GetChild(i);

            if (_child.gameObject.activeInHierarchy)
            {
                EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, _child.gameObject);
                return;
            }
        }
    }

    protected virtual void onActiveInputDeviceChanged(InputDeviceTypes types) { }
    protected InputDeviceTypes getActiveInputDevice()
    {
        var _uiEventSystem = UIEventSystemComponent.Instance;
        if (_uiEventSystem != null)
            return _uiEventSystem.ActiveInputDevice;

        return InputDeviceTypes.Inactive;
    }
}