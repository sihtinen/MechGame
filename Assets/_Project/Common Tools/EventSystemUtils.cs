using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class EventSystemUtils
{
    private static List<ISelectHandler> m_selectHandlers = new List<ISelectHandler>();

    public static void SetSelectedObjectWithManualCall(GameObject go, bool isDelayed = true)
    {
        if (isDelayed)
        {
            EventSystem.current.StartCoroutine(setSelectedObject(go));
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);

        if (go != null)
        {
            m_selectHandlers.Clear();
            go.GetComponentsInChildren(includeInactive: true, m_selectHandlers);

            for (int i = 0; i < m_selectHandlers.Count; i++)
                m_selectHandlers[i].OnSelect(null);
        }
    }

    private static IEnumerator setSelectedObject(GameObject go)
    {
        yield return null;
        SetSelectedObjectWithManualCall(go, isDelayed: false);
    }
}
