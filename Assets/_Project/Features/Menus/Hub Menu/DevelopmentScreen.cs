using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentScreen : UIScreen<DevelopmentScreen>
{
    [Header("Development Screen Settings")]
    [SerializeField] private UITabManager m_tabManager = null;

    protected override void Start()
    {
        base.Start();

        m_tabManager.OpenTab(0);
    }
}