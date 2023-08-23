using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectionHighlight : MonoBehaviour, 
    ISelectHandler, 
    IDeselectHandler, 
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private GameObject m_onSelectedElement = null;
    [SerializeField] private Image m_onSelectedImage = null;

    [Header("Unity Events")]
    public UnityEvent OnSelected = new();
    public UnityEvent OnDeselected = new();

    private bool m_isSelected = false;

    private void Start()
    {
        setHighlightActive(false);
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            setHighlightActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (m_isSelected)
            return;

        m_isSelected = true;

        setHighlightActive(true);

        OnSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (m_isSelected == false)
            return;

        m_isSelected = false;

        setHighlightActive(false);

        OnDeselected?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void setHighlightActive(bool isActive)
    {
        if (m_onSelectedElement != null)
            m_onSelectedElement.SetActiveOptimized(isActive);

        if (m_onSelectedImage != null)
        {
            var _col = m_onSelectedImage.color;
            _col.a = isActive ? 1 : 0;
            m_onSelectedImage.color = _col;
        }
    }
}