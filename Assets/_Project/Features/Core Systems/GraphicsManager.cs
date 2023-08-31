using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : SingletonBehaviour<GraphicsManager>
{
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        Application.targetFrameRate = 144;
    }
}
