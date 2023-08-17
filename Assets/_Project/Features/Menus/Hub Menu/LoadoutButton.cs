using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadoutButton : PoolableBehaviour<LoadoutButton>
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_nameText = null;

    protected override void resetAndClearBindings()
    {

    }

    internal void Populate(MechLoadout mechLoadout, bool isSelected)
    {
        m_nameText.SetText(mechLoadout.LoadoutName);
    }
}
