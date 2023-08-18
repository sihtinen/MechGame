using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManageLoadoutPanel : SingletonBehaviour<ManageLoadoutPanel>
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_editLoadoutNameText = null;
    [SerializeField] private GameObject m_controllerActiveObject = null;
    [SerializeField] private Button m_setActiveButton = null;
    [SerializeField] private Button m_deleteButton = null;

    public void OpenPanel(MechLoadout loadout, bool isSelected, bool canBeDeleted)
    {
        m_editLoadoutNameText.SetText(loadout.LoadoutName);

        m_setActiveButton.interactable = isSelected == false;
        m_deleteButton.interactable = canBeDeleted;

        gameObject.SetActiveOptimized(true);

        ResetControllerSelection();
    }

    public void ClosePanel()
    {
        gameObject.SetActiveOptimized(false);
    }

    public void ResetControllerSelection()
    {
        StartCoroutine(coroutine_setSelectedObjectDelayed());
    }

    private IEnumerator coroutine_setSelectedObjectDelayed()
    {
        yield return null;
        EventSystemUtils.SetSelectedObjectWithManualCall(m_controllerActiveObject);
    }
}