using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPanel : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private RectTransform m_verticalGroupParent = null;

    private List<DataPanelTextElement> m_activeTextElements = new();
    private List<HorizontalDivider> m_activeHorizontalDividers = new();

    public void Clear()
    {
        for (int i = m_activeTextElements.Count; i --> 0;)
            m_activeTextElements[i].ResetAndReturnToPool();

        for (int i = m_activeHorizontalDividers.Count; i-- > 0;)
            m_activeHorizontalDividers[i].ResetAndReturnToPool();

        m_activeTextElements.Clear();
        m_activeHorizontalDividers.Clear();
    }

    public DataPanelTextElement CreateTextElement()
    {
        var _textElement = DataPanelTextElementPool.Get();
        _textElement.transform.SetParent(m_verticalGroupParent);
        _textElement.gameObject.SetActiveOptimized(true);
        m_activeTextElements.Add(_textElement);
        return _textElement;
    }

    public HorizontalDivider CreateDivider()
    {
        var _divider = HorizontalDividerPool.Get();
        _divider.transform.SetParent(m_verticalGroupParent);
        _divider.gameObject.SetActiveOptimized(true);
        m_activeHorizontalDividers.Add(_divider);
        return _divider;
    }

    public void PopulateWithEmptyData()
    {
        CreateTextElement().Initialize("-EMPTY-", 24);
        CreateDivider().SetPreferredHeight(14);
        CreateTextElement().Initialize("No equipment installed in this slot", 18);
    }
}
