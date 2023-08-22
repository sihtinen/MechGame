using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputGuideElementPool : PoolableBehaviourPool<InputGuideElement>
{
    [Header("Input Action References")]
    [SerializeField] protected InputActionReference m_submitInputActionRef = null;
    [SerializeField] protected InputActionReference m_cancelInputActionRef = null;

    public static void CreateGuide_SubmitButton(string displayText, InputDeviceTypes deviceType)
    {
        var _instance = m_instance as InputGuideElementPool;
        var _newButton = Get();
        _newButton.Initialize(displayText, "LMB", deviceType, _instance.m_submitInputActionRef);
    }

    public static void CreateGuide_CancelButton(string displayText, InputDeviceTypes deviceType)
    {
        var _instance = m_instance as InputGuideElementPool;
        var _newButton = Get();
        _newButton.Initialize(displayText, "ESC", deviceType, _instance.m_cancelInputActionRef);
    }
}