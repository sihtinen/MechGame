using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentTypeHeaderElement : PoolableBehaviour<EquipmentTypeHeaderElement>
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_text = null;

    public void SetText(string text) => m_text.SetText(text);

    protected override void resetAndClearBindings()
    {

    }
}
