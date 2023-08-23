using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalDivider : PoolableBehaviour<HorizontalDivider>
{
    private LayoutElement m_layoutElement = null;

    protected override void resetAndClearBindings()
    {

    }

    public void SetPreferredHeight(float height)
    {
        (transform as RectTransform).SetHeight(height);

        if (m_layoutElement == null)
            m_layoutElement = gameObject.AddComponent<LayoutElement>();

        m_layoutElement.preferredHeight = height;
    }
}