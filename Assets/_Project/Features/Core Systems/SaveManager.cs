using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tensori.SaveSystem;

public class SaveManager : SingletonBehaviour<SaveManager>
{
    [Header("Save Manager Settings")]
    [SerializeField] private string m_saveFileName = "save_slot_";
    [SerializeField] private double m_saveFileVersion = 1.0;
    public double SaveFileVersion => m_saveFileVersion;

    [Header("Runtime Parameters")]
    [NonEditable, SerializeField] private string m_currentSaveSlotId = "editor";

    private SaveData m_currentSaveData = null;
    public SaveData CurrentSave => m_currentSaveData;

    protected override void Awake()
    {
        base.Awake();

        if (this != Instance)
            return;

        m_currentSaveData = new SaveData(m_saveFileVersion);

        if (Application.isEditor)
        {
            bool _editorSaveFound = LoadSaveData();

            if (_editorSaveFound == false)
                SaveData();
        }
    }

    public void SaveData(string saveSlotID = null)
    {
        if (saveSlotID != null)
            m_currentSaveSlotId = saveSlotID;

        string _saveFileName = m_saveFileName + m_currentSaveSlotId;
        SaveSystemUtils.SaveToFile(_saveFileName, m_currentSaveData);
    }

    public bool LoadSaveData(string saveSlotID = null)
    {
        if (saveSlotID != null)
            m_currentSaveSlotId = saveSlotID;

        string _saveFileName = m_saveFileName + m_currentSaveSlotId;
        return SaveSystemUtils.LoadFromFile(_saveFileName, ref m_currentSaveData, SaveSystemUtils.SaveFileFormat.Json);
    }
}