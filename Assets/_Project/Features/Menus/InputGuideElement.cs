using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputGuideElement : PoolableBehaviour<InputGuideElement>
{
    [SerializeField] private TMP_Text m_text = null;
    [SerializeField] private Image m_image = null;

    protected override void resetAndClearBindings()
    {

    }

    public void Initialize(string text, string keyboardAbbreviation, InputDeviceTypes deviceType, InputActionReference inputActionRef)
    {
        var _eventSys = UIEventSystemComponent.Instance;
        var _icon = _eventSys.ControllerButtonIconDatabaseAsset.GetIcon(deviceType, inputActionRef);

        if (_icon == null)
        {
            text += $" ({keyboardAbbreviation})";
            m_image.gameObject.SetActiveOptimized(false);
        }
        else
        {
            m_image.overrideSprite = _icon;
            m_image.gameObject.SetActiveOptimized(true);
        }

        m_text.SetText(text);

        gameObject.SetActiveOptimized(true);
    }
}
