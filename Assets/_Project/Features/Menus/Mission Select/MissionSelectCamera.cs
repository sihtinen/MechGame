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
        _missionSelectScreen.OnSelectedMissionUpdated += this.onSelectedMissionUpdated;
    }

    private void OnDestroy()
    {
        if (Application.isPlaying == false)
            return;

        var _missionSelectScreen = MissionSelectScreen.Instance;
        if (_missionSelectScreen != null)
            _missionSelectScreen.OnSelectedMissionUpdated -= this.onSelectedMissionUpdated;
    }

    private void onSelectedMissionUpdated()
    {
        var _missionSelectScreen = MissionSelectScreen.Instance;

        bool _isMissionSelected = _missionSelectScreen.ActiveMission != null;

        bool _isCameraActive = m_isCameraActiveWhenMissionSelected == _isMissionSelected;

        if (_isMissionSelected)
            m_cameraTarget.transform.position = _missionSelectScreen.ActiveMission.transform.position;

        m_vcam.enabled = _isCameraActive;
    }

    private void LateUpdate()
    {

    }
}