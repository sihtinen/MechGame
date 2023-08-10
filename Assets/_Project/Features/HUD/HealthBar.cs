using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : PoolableBehaviour<HealthBar>
{
    [Header("Object References")]
    [SerializeField] private HealthBarTick m_tickPrefab = null;

    private RectTransform m_rectTransform = null;

    private void Awake()
    {
        
    }

    protected override void resetAndClearBindings()
    {

    }
}
