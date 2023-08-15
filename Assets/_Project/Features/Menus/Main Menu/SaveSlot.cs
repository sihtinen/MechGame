using System.Collections;
using System.Collections.Generic;
using Tensori.SaveSystem;
using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private GameObject m_emptySlotIndicator = null;
    [SerializeField] private TMP_Text m_underText = null;

    public void BindToSaveSlot(SaveData saveData)
    {
        m_emptySlotIndicator.SetActive(false);
        m_underText.enabled = false;
    }
}
