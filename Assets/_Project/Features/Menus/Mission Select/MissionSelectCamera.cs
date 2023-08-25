using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectCamera : MonoBehaviour
{
    [SerializeField] private bool m_isCameraActiveWhenMissionSelected = true;

    [Header("Object References")]
    [SerializeField] private Transform m_cameraTarget = null;

    private CinemachineVirtualCamera m_vcam = null;
    private CinemachineFramingTransposer m_framingTransposer = null;

    private void Awake()
    {
        TryGetComponent(out m_vcam);
        m_framingTransposer = m_vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        var _missionSelectScreen = MissionSelectScreen.Instance;
        _missionSelectScreen.OnHighlightMissionUpdated += this.updateCameraState;
        _missionSelectScreen.OnSelectedMissionUpdated += this.updateCameraState;
    }

    private void OnDestroy()
    {
        if (Application.isPlaying == false)
            return;

        var _missionSelectScreen = MissionSelectScreen.Instance;
        if (_missionSelectScreen != null)
        {
            _missionSelectScreen.OnHighlightMissionUpdated -= this.updateCameraState;
            _missionSelectScreen.OnSelectedMissionUpdated -= this.updateCameraState;
        }
    }

    private void updateCameraState()
    {
        var _missionSelectScreen = MissionSelectScreen.Instance;
        var _eventSystem = UIEventSystemComponent.Instance;

        if (_eventSystem.ActiveInputDevice == InputDeviceTypes.KeyboardAndMouse)
        {
            if (_missionSelectScreen.ActiveMission != null)
                m_cameraTarget.transform.position = _missionSelectScreen.ActiveMission.transform.position;
        }
        else
        {
            if (_missionSelectScreen.HighlightMission != null)
                m_cameraTarget.transform.position = _missionSelectScreen.HighlightMission.transform.position;
        }

        bool _isMissionSelected = _missionSelectScreen.ActiveMission != null;
        bool _isCameraActive = m_isCameraActiveWhenMissionSelected == _isMissionSelected;

        m_vcam.enabled = _isCameraActive;
    }
}