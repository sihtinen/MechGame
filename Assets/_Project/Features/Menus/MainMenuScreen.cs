using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : UIScreen<MainMenuScreen>
{
    protected override void Start()
    {
        base.Start();

        this.Open();
    }

    public void Button_Start()
    {
        this.Close();

        SaveSlotSelectionScreen.Instance.Open();
    }

    public void Button_Options()
    {

    }

    public void Button_Exit()
    {
        this.Close();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}