using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tensori.SaveSystem;
using System;

public class SaveManager : SingletonBehaviour<SaveManager>
{
    [Header("Save Manager Settings")]
    [SerializeField] private string m_saveFileName = "save_slot_";
    [SerializeField] private double m_saveFileVersion = 1.0;
    [SerializeField] private List<SavePreProcessor> m_savePreProcessors = new List<SavePreProcessor>();

    public double SaveFileVersion => m_saveFileVersion;

    [Header("Runtime Parameters")]
    [NonEditable, SerializeField] private string m_currentSaveSlotId = "editor";

    private SaveData m_currentSaveData = null;
    public SaveData CurrentSave => m_currentSaveData;

    public event Action OnSaveLoaded = null;

    protected override void Awake()
    {
        base.Awake();

        if (this != Instance)
            return;

        m_currentSaveData = new SaveData(m_saveFileVersion);

        if (Application.isEditor)
        {
            LoadSaveData();
            SaveData();
        }
    }

    public void SaveData(string saveSlotID = null)
    {
        if (saveSlotID != null)
            m_currentSaveSlotId = saveSlotID;

        for (int i = 0; i < m_savePreProcessors.Count; i++)
            m_savePreProcessors[i].PreProcess(m_currentSaveData);

        string _saveFileName = m_saveFileName + m_currentSaveSlotId;
        SaveSystemUtils.SaveToFile(_saveFileName, m_currentSaveData);
    }

    public bool LoadSaveData(string saveSlotID = null)
    {
        if (saveSlotID != null)
            m_currentSaveSlotId = saveSlotID;

        string _saveFileName = m_saveFileName + m_currentSaveSlotId;
        bool _saveFileLoaded = SaveSystemUtils.LoadFromFile(_saveFileName, ref m_currentSaveData, SaveSystemUtils.SaveFileFormat.Json);

        if (_saveFileLoaded == false)
        {
            m_currentSaveData.Version = m_saveFileVersion;
            m_currentSaveData.ClearRegisteredData();
        }

        for (int i = 0; i < m_savePreProcessors.Count; i++)
            m_savePreProcessors[i].PreProcess(m_currentSaveData);

        if (_saveFileLoaded)
            OnSaveLoaded?.Invoke();

        return _saveFileLoaded;
    }

    [ContextMenu("Console Log Save Data")]
    public void Editor_ConsoleLogSaveData()
    {
        if (m_currentSaveData != null)
            Debug.Log(m_currentSaveData.ToString());
    }
}