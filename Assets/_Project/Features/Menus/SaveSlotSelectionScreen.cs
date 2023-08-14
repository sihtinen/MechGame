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
        if (IsOpened == false || wasOpenedThisFrame())
            return;

        this.Close();

        MainMenuScreen.Instance.Open();
    }

    public void Button_Slot(int slotIndex)
    {
        this.Close();

        ConfirmationScreen.Instance.BuildContent(
            header: "Create new save?",
            description: $"Do you want to create a new save file in slot {slotIndex+1}?\nAll progress will be saved automatically in the release version.",
            onAccept: this.onCreateNewSaveAccept,
            onCancel: this.onCreateNewSaveCancel);

        ConfirmationScreen.Instance.Open();
    }

    private void onCreateNewSaveAccept()
    {
        ConfirmationScreen.Instance.Close();
    }

    private void onCreateNewSaveCancel()
    {
        ConfirmationScreen.Instance.Close();

        this.Open();
    }
}
