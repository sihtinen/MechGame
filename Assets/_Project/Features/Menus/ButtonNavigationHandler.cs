using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ButtonNavigationHandler : MonoBehaviour
{
    private CanvasGroup m_canvasGroup = null;

    private void OnEnable()
    {
        if (Application.isPlaying == false)
            return;

        if (m_canvasGroup == null)
            TryGetComponent(out m_canvasGroup);

        m_canvasGroup.interactable = true;
    }

    private void OnDisable()
    {
        if (Application.isPlaying == false)
            return;

        if (m_canvasGroup == null)
            TryGetComponent(out m_canvasGroup);

        m_canvasGroup.interactable = false;
    }
}
