using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentScreen : UIScreen<DevelopmentScreen>
{
    [SerializeField] private UITabManager m_tabManager = null;

    protected override void onOpened()
    {
        base.onOpened();

        m_tabManager.OpenTab(0);

        updateInputGuides(UIEventSystemComponent.Instance.ActiveInputDevice);
    }

    protected override void onInputDeviceChanged(InputDeviceTypes deviceType)
    {
        base.onInputDeviceChanged(deviceType);

        if (IsOpened == false)
            return;

        updateInputGuides(deviceType);
    }

    protected void updateInputGuides(InputDeviceTypes deviceType)
    {
        InputGuideElementPool.ResetUsedObjects();
        InputGuideElementPool.CreateGuide_SubmitButton("Select", deviceType);
        InputGuideElementPool.CreateGuide_CancelButton("Return", deviceType);
    }

    public void OpenTab(int index) => m_tabManager.OpenTab(index);
}