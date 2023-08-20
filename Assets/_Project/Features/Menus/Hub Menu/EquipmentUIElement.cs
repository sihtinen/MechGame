using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentUIElement : PoolableBehaviour<EquipmentUIElement>
{
    [SerializeField] private TMP_Text m_nameText = null;
    [SerializeField] private TMP_Text m_typeText = null;

    private Equipment m_equipment = null;

    protected override void resetAndClearBindings()
    {
        m_equipment = null;
    }

    public void BindToEquipment(Equipment equipment)
    {
        m_equipment = equipment;

        m_nameText.SetText(m_equipment.DisplayName);
        m_typeText.SetText(m_equipment.Category.DisplayName);
    }
}