using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HubScreen : UIScreen<HubScreen>
{
    protected override void Start()
    {
        base.Start();

        this.Open();
    }

    public void Button_Missions()
    {

    }

    public void Button_Development()
    {
        this.Close();

        DevelopmentScreen.Instance.Open();
    }

    public void Button_Settings()
    {

    }

    public void Button_Exit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    protected override void onOpened()
    {
        base.onOpened();

        updateInputGuides(UIEventSystemComponent.Instance.ActiveInputDevice);
    }

    protected override void onInputDeviceChanged(InputDeviceTypes deviceType)
    {
        base.onInputDeviceChanged(deviceType);

        if (IsOpened == false)
            return;

        updateInputGuides(deviceType);
    }

    private void updateInputGuides(InputDeviceTypes deviceType)
    {
        InputGuideElementPool.ResetUsedObjects();
        InputGuideElementPool.CreateGuide_SubmitButton("Select", deviceType);
    }
}