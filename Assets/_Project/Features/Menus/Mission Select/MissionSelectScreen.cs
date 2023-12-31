using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissionSelectScreen : UIScreen<MissionSelectScreen>
{
    [Header("Runtime Parameters")]
    [NonEditable] public MissionUIElement HighlightMission = null;
    [NonEditable] public MissionUIElement ActiveMission = null;

    [Header("Visual Settings")]
    [SerializeField] private float m_detailsPanelSmoothTime = 0.2f;
    [SerializeField] private float m_detailsPanelMaxSpeed = 0.2f;
    [SerializeField] private Vector2 m_detailsPanelInvisibleAnchorMin = Vector2.zero;
    [SerializeField] private Vector2 m_detailsPanelInvisibleAnchorMax = Vector2.zero;

    [Header("Object References")]
    [SerializeField] private GameObject m_missionSelectWorldObjects = null;
    [SerializeField] private MissionUIElement m_gamepadFirstMissionElement = null;
    [SerializeField] private MissionDetailsPanel m_missionDetailsPanel = null;
    [Space]
    [SerializeField] private InputActionReference m_submitInputActionRef = null;
    [SerializeField] private InputActionReference m_cancelInputActionRef = null;
    [SerializeField] private InputActionReference m_navigateLeftInputActionRef = null;
    [SerializeField] private InputActionReference m_navigateRightInputActionRef = null;
    [SerializeField] private InputActionReference m_navigateUpInputActionRef = null;
    [SerializeField] private InputActionReference m_navigateDownInputActionRef = null;

    private float m_detailsPanelVisibility = 0;
    private float m_detailsPanelVelocity = 0;

    private Vector2 m_offsetMin, m_offsetMax;

    public event Action OnHighlightMissionUpdated = null;
    public event Action OnSelectedMissionUpdated = null;

    protected override void Start()
    {
        base.Start();

        m_offsetMin = m_missionDetailsPanel.RectTransformComponent.offsetMin;
        m_offsetMax = m_missionDetailsPanel.RectTransformComponent.offsetMax;

        setInputActionEnabled(m_navigateLeftInputActionRef, true);
        setInputActionEnabled(m_navigateRightInputActionRef, true);
        setInputActionEnabled(m_navigateUpInputActionRef, true);
        setInputActionEnabled(m_navigateDownInputActionRef, true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        setInputActionEnabled(m_navigateLeftInputActionRef, false);
        setInputActionEnabled(m_navigateRightInputActionRef, false);
        setInputActionEnabled(m_navigateUpInputActionRef, false);
        setInputActionEnabled(m_navigateDownInputActionRef, false);
    }

    private void setInputActionEnabled(InputActionReference inputActionRef, bool isEnabled)
    {
        if (isEnabled)
        {
            if (inputActionRef.action.enabled == false)
                inputActionRef.action.Enable();
        }
        else
        {
            if (inputActionRef.action.enabled)
                inputActionRef.action.Disable();
        }
    }


    protected override void onOpened()
    {
        base.onOpened();

        SetHighlightMission(null);
        SetActiveMission(null);

        m_missionSelectWorldObjects.SetActiveOptimized(true);

        if (UIEventSystemComponent.Instance.ActiveInputDevice != InputDeviceTypes.KeyboardAndMouse)
            SetHighlightMission(m_gamepadFirstMissionElement);

        InputGuideElementPool.ResetUsedObjects();
    }

    protected override void onClosed()
    {
        base.onClosed();

        m_missionSelectWorldObjects.SetActiveOptimized(false);
    }

    protected override void onInputDeviceChanged(InputDeviceTypes deviceType)
    {
        base.onInputDeviceChanged(deviceType);

        if (IsOpened == false)
            return;

        if (deviceType != InputDeviceTypes.KeyboardAndMouse)
            SetHighlightMission(m_gamepadFirstMissionElement);
    }

    private void Update()
    {
        if (IsOpened == false)
            return;

        pollInput();

        updateDetailsPanelPosition();
    }

    private void pollInput()
    {
        if (m_cancelInputActionRef.action.WasPressedThisFrame())
        {
            if (ActiveMission != null)
                SetActiveMission(null);
            else
            {
                Close();
                HubScreen.Instance.Open();
            }

            return;
        }

        if (HighlightMission == null)
            return;

        if (m_submitInputActionRef.action.WasPerformedThisFrame())
        {
            if (ActiveMission == null)
                SetActiveMission(HighlightMission);
            else
                Button_DetailsPanelAccept();

            return;
        }

        if (m_navigateLeftInputActionRef.action.WasPerformedThisFrame() && HighlightMission.ConnectionLeft != null)
            SetHighlightMission(HighlightMission.ConnectionLeft);
        else if (m_navigateRightInputActionRef.action.WasPerformedThisFrame() && HighlightMission.ConnectionRight != null)
            SetHighlightMission(HighlightMission.ConnectionRight);
        else if (m_navigateUpInputActionRef.action.WasPerformedThisFrame() && HighlightMission.ConnectionUp != null)
            SetHighlightMission(HighlightMission.ConnectionUp);
        else if (m_navigateDownInputActionRef.action.WasPerformedThisFrame() && HighlightMission.ConnectionDown != null)
            SetHighlightMission(HighlightMission.ConnectionDown);
    }

    private void updateDetailsPanelPosition()
    {
        float _targetVisibility = 0;

        if (ActiveMission != null)
            _targetVisibility = 1;

        m_detailsPanelVisibility = Mathf.SmoothDamp(
            current: m_detailsPanelVisibility, 
            target: _targetVisibility, 
            currentVelocity: ref m_detailsPanelVelocity, 
            smoothTime: m_detailsPanelSmoothTime, 
            maxSpeed: m_detailsPanelMaxSpeed);

        var _panelRectTransform = m_missionDetailsPanel.RectTransformComponent;

        _panelRectTransform.anchorMin = Vector2.Lerp(m_detailsPanelInvisibleAnchorMin, new Vector2(0, 0), m_detailsPanelVisibility);
        _panelRectTransform.anchorMax = Vector2.Lerp(m_detailsPanelInvisibleAnchorMax, new Vector2(0.3f, 1), m_detailsPanelVisibility);

        _panelRectTransform.offsetMin = m_offsetMin;
        _panelRectTransform.offsetMax = m_offsetMax;
    }

    public void SetHighlightMission(MissionUIElement missionElement)
    {
        if (missionElement == HighlightMission)
            return;

        if (HighlightMission != null)
            HighlightMission.onHoverEnd();

        HighlightMission = missionElement;

        if (HighlightMission != null)
            HighlightMission.onHoverStart();

        OnHighlightMissionUpdated?.Invoke();

        if (UIEventSystemComponent.Instance.ActiveInputDevice != InputDeviceTypes.KeyboardAndMouse && ActiveMission != null)
            SetActiveMission(HighlightMission);
    }

    public void SetActiveMission(MissionUIElement missionElement)
    {
        if (missionElement == ActiveMission)
            return;

        if (ActiveMission != null)
            ActiveMission.IsSelected = false;

        ActiveMission = missionElement;

        if (ActiveMission != null)
            ActiveMission.IsSelected = true;

        OnSelectedMissionUpdated?.Invoke();
    }

    public void Button_DetailsPanelAccept()
    {
        if (ActiveMission != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(ActiveMission.Mission.Scene.ScenePath, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void Button_DetailsPanelClose()
    {
        SetActiveMission(null);
    }
}