using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class UITabManager : MonoBehaviour
{
    [SerializeField] private List<UITab> m_tabs = new List<UITab>();

    private int m_currentTabIndex;

    private InputAction m_previousTabInputAction = null;
    private InputAction m_nextTabInputAction = null;

    private void Start()
    {
        for (int i = 0; i < m_tabs.Count; i++)
            m_tabs[i].Close();

        var _uiEventComponent = UIEventSystemComponent.Instance;
        if (_uiEventComponent != null)
        {
            var _actionMap = _uiEventComponent.UIActionMap;

            m_previousTabInputAction = _actionMap.FindAction("TabPrevious");
            m_previousTabInputAction.started += this.onPreviousTabInput;
            if (m_previousTabInputAction.enabled == false) m_previousTabInputAction.Enable();

            m_nextTabInputAction = _actionMap.FindAction("TabNext");
            m_nextTabInputAction.started += this.onNextTabInput;
            if (m_nextTabInputAction.enabled == false) m_nextTabInputAction.Enable();
        }
    }

    private void OnDestroy()
    { 
        if (m_previousTabInputAction != null)
        {
            m_previousTabInputAction.started -= this.onPreviousTabInput;
            if (m_previousTabInputAction.enabled) m_previousTabInputAction.Disable();
            m_previousTabInputAction = null;
        }

        if (m_nextTabInputAction != null)
        {
            m_nextTabInputAction.started -= this.onNextTabInput;
            if (m_nextTabInputAction.enabled) m_nextTabInputAction.Disable();
            m_nextTabInputAction = null;
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
