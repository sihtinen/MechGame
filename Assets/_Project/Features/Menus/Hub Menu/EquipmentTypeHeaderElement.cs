using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentTypeHeaderElement : PoolableBehaviour<EquipmentTypeHeaderElement>
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_text = null;

    public void SetText(string text) => m_text.SetText(text);
    public void SetHeight(float height) => (transform as RectTransform).SetHeight(height);

    protected override void resetAndClearBindings()
    {

    }
}
