using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectionHighlight : MonoBehaviour, 
    ISelectHandler, 
    IDeselectHandler, 
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private GameObject m_onSelectedElement = null;

    private void Start()
    {
        if (m_onSelectedElement != null)
            m_onSelectedElement.SetActiveOptimized(false);
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            m_onSelectedElement.SetActiveOptimized(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (m_onSelectedElement != null)
            m_onSelectedElement.SetActiveOptimized(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (m_onSelectedElement != null)
            m_onSelectedElement.SetActiveOptimized(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystemUtils.SetSelectedObjectWithManualCall(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}