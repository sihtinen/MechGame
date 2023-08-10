using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechLoader : MonoBehaviour
{
    [SerializeField] private MechController m_targetMech = null;
    [SerializeField] private MechController.InitializeSettings m_initializeSettings = new MechController.InitializeSettings();

    private void Start()
    {
        if (m_targetMech != null)
            m_targetMech.InitializeGameplay(m_initializeSettings);
    }
}
