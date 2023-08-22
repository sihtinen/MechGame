using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCategoryButton : PoolableBehaviour<EquipmentCategoryButton>
{
    [SerializeField] private TMP_Text m_text = null;
    [SerializeField] private Image m_image = null;

    private EquipmentCategory m_category = null;
    private Action m_onClickedCallback = null;

    protected override void resetAndClearBindings()
    {
        m_category = null;
        m_onClickedCallback = null;
    }

    public void Button_Clicked()
    {
        m_onClickedCallback?.Invoke();
    }
}
