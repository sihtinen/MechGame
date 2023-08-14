using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConfirmationScreen : UIScreen<ConfirmationScreen>
{
    [SerializeField] private TMP_Text m_headerText = null;
    [SerializeField] private TMP_Text m_descriptionText = null;

    private InputAction m_acceptInputAction = null;
    private InputAction m_cancelInputAction = null;
    private Action m_onAcceptEvent = null;
    private Action m_onCancelEvent = null;

    protected override void Start()
    {
        base.Start();

        UIEventSystemComponent _uiEventComponent = UIEventSystemComponent.Instance;
        if (_uiEventComponent != null)
        {
            var _actionMap = _uiEventComponent.UIActionMap;

            m_acceptInputAction = _actionMap.FindAction("Submit");
            m_acceptInputAction.performed += this.onAcceptInputPerformed;

            m_cancelInputAction = _actionMap.FindAction("Cancel");
            m_cancelInputAction.performed += this.onCancelInputPerformed;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_acceptInputAction != null)
        {
            m_acceptInputAction.performed -= this.onAcceptInputPerformed;
            m_acceptInputAction = null;
        }

        if (m_cancelInputAction != null)
        {
            m_cancelInputAction.performed -= this.onCancelInputPerformed;
            m_cancelInputAction = null;
        }

        m_onAcceptEvent = null;
        m_onCancelEvent = null;
    }

    private void onAcceptInputPerformed(InputAction.CallbackContext context)
    {
        if (IsOpened == false || wasOpenedThisFrame())
            return;

        Button_Accept();
    }

    private void onCancelInputPerformed(InputAction.CallbackContext context)
    {
        if (IsOpened == false || wasOpenedThisFrame())
            return;

        Button_Cancel();
    }

    public void BuildContent(string header, string description, Action onAccept, Action onCancel)
    {
        m_headerText.SetText(header);
        m_descriptionText.SetText(description);

        m_onAcceptEvent = onAccept;
        m_onCancelEvent = onCancel;
    }

    public void Button_Accept()
    {
        m_onAcceptEvent?.Invoke();
    }

    public void Button_Cancel()
    {
        m_onCancelEvent?.Invoke();
    }
}