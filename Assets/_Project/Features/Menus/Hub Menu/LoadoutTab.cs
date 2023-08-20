using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LoadoutTab : UITab
{
    [Header("Object References")]
    [SerializeField] private RectTransform m_loadoutButtonGroupTransform = null;
    [SerializeField] private UISelectionHighlight m_createNewLoadoutButton = null;
    [SerializeField] private RectTransform m_dividerElement = null;
    [SerializeField] private RectTransform m_selectionBackButtonElement = null;
    [SerializeField] private GameObject m_emptyLoadoutSlotPrefab = null;
    [Space]
    [SerializeField] private MechLoadout m_selectedLoadoutAsset = null;
    [Space]
    [SerializeField] private RectTransform m_loadoutSelectionPanel = null;
    [Space]
    [SerializeField] private ManageLoadoutPanel m_manageLoadoutPanel = null;

    private int m_editLoadoutIndex = -1;
    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new MechLoadout.MechLoadoutListSerialized();

    private InputAction m_cancelAction = null;

    private List<GameObject> m_emptyLoadoutSlotElements = new List<GameObject>();
    private List<LoadoutButton> m_activeLoadoutSelectionButtons = new List<LoadoutButton>();

    private int m_maxLoadouts = 6;

    private void Awake()
    {
        for (int i = 0; i < m_maxLoadouts; i++)
        {
            var _newEmptyLoadoutElement = Instantiate(m_emptyLoadoutSlotPrefab, m_loadoutButtonGroupTransform);
            _newEmptyLoadoutElement.SetActiveOptimized(false);
            m_emptyLoadoutSlotElements.Add(_newEmptyLoadoutElement);
        }
    }

    private void Start()
    {
        var _uiEventSystemComponent = UIEventSystemComponent.Instance;
        if (_uiEventSystemComponent != null)
        {
            _uiEventSystemComponent.OnActiveInputDeviceChanged += this.onActiveInputDeviceChanged;

            m_cancelAction = _uiEventSystemComponent.UIActionMap.FindAction("Cancel");
            m_cancelAction.started += this.onCancelInput;
        }
    }

    private void OnDestroy()
    {
        var _uiEventSystemComponent = UIEventSystemComponent.Instance;
        if (_uiEventSystemComponent != null)
            _uiEventSystemComponent.OnActiveInputDeviceChanged -= this.onActiveInputDeviceChanged;

        if (m_cancelAction != null)
        {
            m_cancelAction.started -= this.onCancelInput;
            m_cancelAction = null;
        }
    }

    private void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        updateSelectedObject();
    }

    private void onCancelInput(InputAction.CallbackContext context)
    {
        if (gameObject == null)
        {
            context.action.performed -= this.onCancelInput;
            return;
        }

        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        if (m_editLoadoutIndex >= 0)
            Button_CloseLoadoutManagement();
        else
            Button_CloseScreen();
    }

    protected override void onOpened()
    {
        base.onOpened();

        m_editLoadoutIndex = -1;

        rebuild();
    }

    private void rebuild()
    {
        m_manageLoadoutPanel.ClosePanel();

        m_activeLoadoutSelectionButtons.Clear();
        LoadoutButtonPool.ResetUsedObjects();

        if (m_editLoadoutIndex < 0)
            buildLoadoutSelectionView();
        else
            buildLoadoutManagementView();

        updatePanels();
        updateSelectedObject();
    }

    private void buildLoadoutSelectionView()
    {
        var _saveData = SaveManager.Instance.CurrentSave;
        var _selectedLoadoutIndex = _saveData.ReadInt(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID).Item2;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        for (int i = m_loadoutList.AllLoadouts.Count; i --> 0;)
        {
            var _loadoutSerialized = m_loadoutList.AllLoadouts[i];
            m_selectedLoadoutAsset.PopulateFromSerializedData(_loadoutSerialized);

            var _newLoadoutButton = LoadoutButtonPool.Get();

            int _index = i;

            _newLoadoutButton.Populate(
                mechLoadout: m_selectedLoadoutAsset,
                isSelected: i == _selectedLoadoutIndex,
                onClick: () => { this.Button_LoadoutSelected(_index); });

            _newLoadoutButton.transform.SetAsFirstSibling();
            _newLoadoutButton.gameObject.SetActiveOptimized(true);

            m_activeLoadoutSelectionButtons.Add(_newLoadoutButton);
        }

        if (m_loadoutList.AllLoadouts.Count < m_maxLoadouts)
        {
            m_createNewLoadoutButton.transform.SetAsLastSibling();
            m_createNewLoadoutButton.gameObject.SetActiveOptimized(true);
        }
        else
            m_createNewLoadoutButton.gameObject.SetActiveOptimized(false);

        int _emptyLoadoutSlotsCount = m_maxLoadouts - m_loadoutList.AllLoadouts.Count - 1;

        for (int i = 0; i < m_maxLoadouts; i++)
        {
            var _element = m_emptyLoadoutSlotElements[i];
            bool _isActive = i < _emptyLoadoutSlotsCount;

            if (_isActive)
                _element.transform.SetAsLastSibling();

            _element.SetActiveOptimized(_isActive);
        }

        m_dividerElement.SetAsLastSibling();
        m_selectionBackButtonElement.SetAsLastSibling();
    }

    private void buildLoadoutManagementView()
    {
        var _saveData = SaveManager.Instance.CurrentSave;
        var _selectedLoadoutIndex = _saveData.ReadInt(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID).Item2;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        var _loadoutSerialized = m_loadoutList.AllLoadouts[m_editLoadoutIndex];
        m_selectedLoadoutAsset.PopulateFromSerializedData(_loadoutSerialized);

        m_createNewLoadoutButton.gameObject.SetActiveOptimized(false);

        m_manageLoadoutPanel.OpenPanel(
            loadout: m_selectedLoadoutAsset,
            isSelected: _selectedLoadoutIndex == m_editLoadoutIndex,
            canBeDeleted: m_loadoutList.AllLoadouts.Count > 1);
    }

    private void updatePanels()
    {
        m_loadoutSelectionPanel.gameObject.SetActiveOptimized(m_editLoadoutIndex < 0);
    }

    private void updateSelectedObject()
    {
        var _uiEventSysComponent = UIEventSystemComponent.Instance;

        if (_uiEventSysComponent.ActiveInputDevice == InputDeviceTypes.KeyboardAndMouse)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        GameObject _selectedObj = null;

        if (m_editLoadoutIndex < 0)
            _selectedObj = m_activeLoadoutSelectionButtons[m_activeLoadoutSelectionButtons.Count - 1].gameObject;
        else
            m_manageLoadoutPanel.ResetControllerSelection();

        EventSystemUtils.SetSelectedObjectWithManualCall(_selectedObj);
    }

    public void Button_LoadoutSelected(int loadoutIndex)
    {
        if (loadoutIndex == m_editLoadoutIndex)
            return;

        m_editLoadoutIndex = loadoutIndex;

        rebuild();
    }

    public void Button_CreateNewLoadout()
    {
        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        m_loadoutList.AllLoadouts.Add(new MechLoadout.MechLoadoutSerialized
        {
            LoadoutName = "New Loadout",
        });

        _saveData.RegisterVariable(SaveIDConstants.LOADOUT_LIST_ID, m_loadoutList);
        SaveManager.Instance.SaveData();

        rebuild();
    }

    public void Button_CloseScreen()
    {
        DevelopmentScreen.Instance.Close();

        HubScreen.Instance.Open();
    }

    public void Button_CloseLoadoutManagement()
    {
        m_editLoadoutIndex = -1;

        rebuild();
    }

    public void Button_SetSelectedLoadoutAsActive()
    {
        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.RegisterVariable(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID, m_editLoadoutIndex);
        SaveManager.Instance.SaveData();

        rebuild();
    }

    public void Button_DeleteSelectedLoadout()
    {
        if (m_loadoutList.AllLoadouts.Count <= 1)
            return;

        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);
        m_loadoutList.AllLoadouts.RemoveAt(m_editLoadoutIndex);
        _saveData.RegisterVariable(SaveIDConstants.LOADOUT_LIST_ID, m_loadoutList);

        int _savedActiveLoadoutIndex = _saveData.ReadInt(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID).Item2;
        if (_savedActiveLoadoutIndex == m_editLoadoutIndex)
            _saveData.RegisterVariable(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID, 0);

        SaveManager.Instance.SaveData();

        m_editLoadoutIndex = -1;
        rebuild();
    }
}