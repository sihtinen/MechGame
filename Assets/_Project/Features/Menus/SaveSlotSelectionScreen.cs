using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveSlotSelectionScreen : UIScreen<SaveSlotSelectionScreen>
{
    private InputAction m_cancelInputAction = null;

    protected override void Start()
    {
        base.Start();

        UIEventSystemComponent _uiEventComponent = UIEventSystemComponent.Instance;
        if (_uiEventComponent != null)
        {
            var _actionMap = _uiEventComponent.UIActionMap;
            m_cancelInputAction = _actionMap.FindAction("Cancel");
            m_cancelInputAction.performed += this.onCancelInputPerformed;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_cancelInputAction != null)
        {
            m_cancelInputAction.performed -= this.onCancelInputPerformed;
            m_cancelInputAction = null;
        }
    }

    private void onCancelInputPerformed(InputAction.CallbackContext context)
    {
        if (IsOpened == false)
            return;

        this.Close();

        MainMenuScreen.Instance.Open();
    }
}
