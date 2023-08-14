using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour, 
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
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}