using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentSlotHeaderElement : PoolableBehaviour<EquipmentSlotHeaderElement>
{
    [SerializeField] private TMP_Text m_text = null;

    public void SetText(string text) => m_text.SetText(text);

    protected override void resetAndClearBindings()
    {

    }
}