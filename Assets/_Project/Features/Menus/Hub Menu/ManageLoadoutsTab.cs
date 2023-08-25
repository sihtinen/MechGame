using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ManageLoadoutsTab : UITab
{
    private static ManageLoadoutsTab m_instance = null;

    [Header("Visual Settings")]
    [SerializeField] private Vector2 m_popupPanel_PosOffset = new Vector2(250, 0);

    [Header("Object References")]
    [SerializeField] private RectTransform m_loadoutButtonGroupTransform = null;
    [SerializeField] private UISelectionHighlight m_createNewLoadoutButton = null;
    [SerializeField] private GameObject m_emptyLoadoutSlotPrefab = null;
    [SerializeField] private MechLoadout m_selectedLoadoutAsset = null;
    [SerializeField] private RectTransform m_popupMenu = null;
    [Space]
    [SerializeField] private Button m_popupButton_Edit = null;
    [SerializeField] private Button m_popupButton_Rename = null;
    [SerializeField] private Button m_popupButton_Duplicate = null;
    [SerializeField] private Button m_popupButton_Delete = null;

    private int m_editLoadoutIndex = -1;
    public static int EditLoadoutIndex => m_instance.m_editLoadoutIndex;
    
    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new MechLoadout.MechLoadoutListSerialized();

    private List<GameObject> m_emptyLoadoutSlotElements = new List<GameObject>();
    private List<LoadoutButton> m_activeLoadoutSelectionButtons = new List<LoadoutButton>();

    private int m_maxLoadouts = 6;

    public override void Initialize()
    {
        base.Initialize();

        m_instance = this;

        for (int i = 0; i < m_maxLoadouts; i++)
        {
            var _newEmptyLoadoutElement = Instantiate(m_emptyLoadoutSlotPrefab, m_loadoutButtonGroupTransform);
            _newEmptyLoadoutElement.SetActiveOptimized(false);
            m_emptyLoadoutSlotElements.Add(_newEmptyLoadoutElement);
        }

        m_popupButton_Edit.onClick.AddListener(this.Button_Edit);
        m_popupButton_Rename.onClick.AddListener(this.Button_Rename);
        m_popupButton_Duplicate.onClick.AddListener(this.Button_Duplicate);
        m_popupButton_Delete.onClick.AddListener(this.Button_Delete);
    }

    protected override void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        updateSelectedObject();
    }

    protected override void onCancelInput(InputAction.CallbackContext context)
    {
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
        m_popupMenu.gameObject.SetActiveOptimized(false);

        rebuild();
    }

    protected override void onClosed()
    {
        base.onClosed();

        m_popupMenu.gameObject.SetActiveOptimized(false);
    }

    private void LateUpdate()
    {
        if (IsOpened == false)
            return;

        if (m_editLoadoutIndex >= 0)
            updatePopupMenuPosition();
    }

    private void rebuild()
    {
        m_activeLoadoutSelectionButtons.Clear();
        LoadoutButtonPool.ResetUsedObjects();

        buildLoadoutSelectionView();
        updatePopupMenuState();
        updateSelectedObject();
    }

    private void updatePopupMenuState()
    {
        bool _isMenuActive = m_editLoadoutIndex >= 0;

        m_popupMenu.gameObject.SetActiveOptimized(_isMenuActive);

        if (_isMenuActive == false)
            return;

        m_popupButton_Delete.interactable = m_loadoutList.AllLoadouts.Count > 1;
    }

    private void buildLoadoutSelectionView()
    {
        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        for (int i = m_loadoutList.AllLoadouts.Count; i --> 0;)
        {
            var _loadoutSerialized = m_loadoutList.AllLoadouts[i];
            m_selectedLoadoutAsset.PopulateFromSerializedData(_loadoutSerialized);

            var _newLoadoutButton = LoadoutButtonPool.Get();

            int _index = i;

            _newLoadoutButton.Populate(
                mechLoadout: m_selectedLoadoutAsset,
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
            _selectedObj = m_popupButton_Edit.gameObject;

        EventSystemUtils.SetSelectedObjectWithManualCall(GetType().Name, _selectedObj);
    }

    public void Button_LoadoutSelected(int loadoutIndex)
    {
        if (loadoutIndex == m_editLoadoutIndex)
            return;

        m_editLoadoutIndex = loadoutIndex;

        rebuild();

        updatePopupMenuPosition();
    }

    private void updatePopupMenuPosition()
    {
        var _buttonElement = m_loadoutButtonGroupTransform.GetChild(m_editLoadoutIndex) as RectTransform;
        var _anchorPos = Camera.main.ScreenToViewportPoint(_buttonElement.position);

        m_popupMenu.anchorMin = _anchorPos;
        m_popupMenu.anchorMax = _anchorPos;
        m_popupMenu.anchoredPosition = m_popupPanel_PosOffset;
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

        m_editLoadoutIndex = m_loadoutList.AllLoadouts.Count - 1;

        rebuild();
    }

    public void Button_CloseScreen()
    {
        DevelopmentScreen.Instance.Close();

        HubScreen.Instance.Open();
    }

    public void Button_Edit()
    {
        DevelopmentScreen.Instance.OpenTab(1);
    }

    public void Button_Rename()
    {

    }

    public void Button_Duplicate()
    {

    }

    public void Button_Delete()
    {
        if (m_loadoutList.AllLoadouts.Count <= 1)
            return;

        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);
        m_loadoutList.AllLoadouts.RemoveAt(m_editLoadoutIndex);
        _saveData.RegisterVariable(SaveIDConstants.LOADOUT_LIST_ID, m_loadoutList);

        SaveManager.Instance.SaveData();

        m_editLoadoutIndex = -1;
        rebuild();
    }

    public void Button_CloseLoadoutManagement()
    {
        m_editLoadoutIndex = -1;

        rebuild();
    }
}