using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutButton : PoolableBehaviour<LoadoutButton>
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_nameText = null;

    private Action m_onClickAction = null;

    protected override void resetAndClearBindings()
    {
        m_onClickAction = null;
    }

    internal void Populate(MechLoadout mechLoadout, Action onClick)
    {
        m_nameText.SetText(mechLoadout.LoadoutName);
        m_onClickAction = onClick;
    }

    public void Button_Clicked()
    {
        m_onClickAction?.Invoke();
    }
}
