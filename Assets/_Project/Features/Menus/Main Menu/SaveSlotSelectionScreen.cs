using System;
using System.Collections;
using System.Collections.Generic;
using Tensori.SaveSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveSlotSelectionScreen : UIScreen<SaveSlotSelectionScreen>
{
    [Header("Save Slots")]
    [SerializeField] private List<SaveSlot> m_saveSlots = new List<SaveSlot>();

    private InputAction m_cancelInputAction = null;

    private List<bool> m_saveSlotsCreated = new List<bool>();
    private List<SaveData> m_saveData = new List<SaveData>();

    private int m_clickedSaveSlotIndex;

    protected override void Start()
    {
        base.Start();

        UIEventSystemComponent _uiEventComponent = UIEventSystemComponent.Instance;
        if (_uiEventComponent != null)
        {
            var _actionMap = _uiEventComponent.UIActionMap;
            m_cancelInputAction = _actionMap.FindAction("Cancel");
            m_cancelInputAction.performed += this.onCancelInputPerformed;
        }

        var _saveManager = SaveManager.Instance;
        if (_saveManager != null)
        {
            for (int i = 0; i < m_saveSlots.Count; i++)
            {
                bool _saveFound = _saveManager.LoadSaveData(i.ToStringMinimalAlloc());

                var _saveData = _saveFound ? 
                    new SaveData(_saveManager.CurrentSave) : 
                    new SaveData(_saveManager.SaveFileVersion);

                if (_saveFound)
                    m_saveSlots[i].BindToSaveSlot(_saveData);

                m_saveData.Add(_saveData);
                m_saveSlotsCreated.Add(_saveFound);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_cancelInputAction != null)
        {
            m_cancelInputAction.performed -= this.onCancelInputPerformed;
            m_cancelInputAction = null;
        }
    }

    private void onCancelInputPerformed(InputAction.CallbackContext context)
    {
        if (IsOpened == false || wasOpenedThisFrame())
            return;

        this.Close();

        MainMenuScreen.Instance.Open();
    }

    public void Button_Slot(int slotIndex)
    {
        m_clickedSaveSlotIndex = slotIndex;

        this.Close();

        bool _saveExists = m_saveSlotsCreated[slotIndex];

        if (_saveExists)
        {
            selectSaveSlotAndLoadNextScene(m_clickedSaveSlotIndex);
        }
        else
        {
            ConfirmationScreen.Instance.BuildContent(
                header: "Create new save?",
                description: $"Do you want to create a new save file in slot {slotIndex + 1}?\nAll progress will be saved automatically in the release version.",
                onAccept: this.onCreateNewSaveAccept,
                onCancel: this.onCreateNewSaveCancel);

            ConfirmationScreen.Instance.Open();
        }
    }

    private void selectSaveSlotAndLoadNextScene(int saveSlotIndex)
    {
        string _saveSlotID = saveSlotIndex.ToStringMinimalAlloc();

        var _saveManager = SaveManager.Instance;
        _saveManager.LoadSaveData(_saveSlotID);
        _saveManager.SaveData(_saveSlotID);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
    }

    private void onCreateNewSaveAccept()
    {
        ConfirmationScreen.Instance.Close();

        selectSaveSlotAndLoadNextScene(m_clickedSaveSlotIndex);
    }

    private void onCreateNewSaveCancel()
    {
        ConfirmationScreen.Instance.Close();

        this.Open();
    }
}
