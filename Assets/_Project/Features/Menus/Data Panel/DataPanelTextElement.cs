using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataPanelTextElement : PoolableBehaviour<DataPanelTextElement>
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_textLeft = null;
    [SerializeField] private TMP_Text m_textRight = null;

    protected override void resetAndClearBindings()
    {

    }

    public void Initialize(string textLeft, float fontSizeLeft = 18)
    {
        m_textLeft.fontSizeMax = fontSizeLeft;
        m_textLeft.SetText(textLeft);

        m_textRight.enabled = false;
    }

    public void Initialize(string textLeft, string textRight, float fontSizeLeft = 18, float fontSizeRight = 18)
    {
        m_textLeft.fontSizeMax = fontSizeLeft;
        m_textLeft.SetText(textLeft);

        m_textRight.fontSizeMax = fontSizeRight;
        m_textRight.SetText(textRight);
        m_textRight.enabled = true;
    }
}
