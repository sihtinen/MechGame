using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UITab : MonoBehaviour
{
    [Header("Base Runtime Parameters")]
    [NonEditable] public bool IsOpened = false;

    [Header("Base Object References")]
    [SerializeField] protected Button m_connectedButton = null;

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
}