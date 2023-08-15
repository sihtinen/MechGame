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

    }

    public void Button_Settings()
    {

    }

    public void Button_Exit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}