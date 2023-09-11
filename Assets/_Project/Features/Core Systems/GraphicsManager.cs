using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : SingletonBehaviour<GraphicsManager>
{
    [SerializeField] private int m_targetFrameRate = 144;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        Application.targetFrameRate = m_targetFrameRate;
    }
}
