using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class UITabManager : MonoBehaviour
{
    [SerializeField] private bool m_enableInput = true;
    [SerializeField] private InputActionReference m_previousTabInputRef = null;
    [SerializeField] private InputActionReference m_nextTabInputRef = null;

    [SerializeField] private List<UITab> m_tabs = new List<UITab>();

    private int m_currentTabIndex = 0;

    private void Start()
    {
        for (int i = 0; i < m_tabs.Count; i++)
            m_tabs[i].Initialize();

        if (m_enableInput)
        {
            m_previousTabInputRef.action.started += this.onPreviousTabInput;
            if (m_previousTabInputRef.action.enabled == false) m_previousTabInputRef.action.Enable();

            m_nextTabInputRef.action.started += this.onNextTabInput;
            if (m_nextTabInputRef.action.enabled == false) m_nextTabInputRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_enableInput)
        {
            m_previousTabInputRef.action.started -= this.onPreviousTabInput;
            m_nextTabInputRef.action.started -= this.onNextTabInput;
        }
    }

    private void onPreviousTabInput(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        OpenTabDirection(-1);
    }

    private void onNextTabInput(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        OpenTabDirection(1);
    }

    public void OpenTab(int tabIndex)
    {
        m_currentTabIndex = tabIndex;

        updateTabOpenStates();
    }

    public void OpenTabDirection(int direction)
    {
        m_currentTabIndex += direction;

        if (m_currentTabIndex < 0)
            m_currentTabIndex += m_tabs.Count;
        else if (m_currentTabIndex >= m_tabs.Count)
            m_currentTabIndex -= m_tabs.Count;

        updateTabOpenStates();
    }

    private void updateTabOpenStates()
    {
        for (int i = 0; i < m_tabs.Count; i++)
        {
            var _tab = m_tabs[i];

            if (i == m_currentTabIndex)
                _tab.Open();
            else
                _tab.Close();
        }
    }
}
