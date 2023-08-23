using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCategoryGroup : PoolableBehaviour<EquipmentCategoryGroup>
{
    private List<EquipmentUIElement> m_activeElements = new();

    protected override void resetAndClearBindings()
    {
        for (int i = m_activeElements.Count; i --> 0;)
            m_activeElements[i].ResetAndReturnToPool();

        m_activeElements.Clear();
    }
}
