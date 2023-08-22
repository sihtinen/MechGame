using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutUtilityTab : UITab
{
    [SerializeField] private RectTransform m_scrollContentRoot = null;

    protected override void onOpened()
    {
        base.onOpened();

        onActiveInputDeviceChanged(getActiveInputDevice());
    }

    protected override void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        if (deviceType != InputDeviceTypes.KeyboardAndMouse)
            setFirstActiveChildAsSelected(m_scrollContentRoot);
    }
}