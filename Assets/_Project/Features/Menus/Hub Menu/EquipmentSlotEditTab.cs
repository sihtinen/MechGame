using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EquipmentSlotEditTab : UITab
{
    [Header("Visual Settings")]
    [SerializeField] private Color m_slotIndicatorColor_Active = Color.white;
    [SerializeField] private Color m_slotIndicatorColor_Inactive = Color.white;

    [Header("Object References")]
    [SerializeField] private RectTransform m_verticalGroupContent = null;
    [SerializeField] private MechLoadout m_editLoadout = null;
    [SerializeField] private EquipmentCollection m_editEquipmentCollection = null;
    [SerializeField] private TMP_Text m_slotNameText = null;
    [SerializeField] private Image m_slotIndicatorPrefab = null;
    [SerializeField] private RectTransform m_slotIndicatorsGroup = null;
    [SerializeField] private DataPanel m_currentItemDataPanel = null;
    [SerializeField] private DataPanel m_selectedItemDataPanel = null;
    [SerializeField] private InputActionReference m_previousTabInputRef = null;
    [SerializeField] private InputActionReference m_nextTabInputRef = null;

    private EquipmentSlotTypes m_slotType = EquipmentSlotTypes.Undefined;

    private MechLoadout.MechLoadoutListSerialized m_loadoutListSerialized = new();
    private EquipmentCollection.EquipmentCollectionSerialized m_unlockedEquipmentSerialized = new();
    private List<EquipmentCategoryGroup> m_activeCategoryGroups = new();
    private List<EquipmentCategory> m_categories = new();
    private List<HorizontalDivider> m_activeDividers = new();
    private List<Image> m_slotIndicatorImages = new();

    public override void Initialize()
    {
        base.Initialize();

        foreach (int i in Enum.GetValues(typeof(EquipmentSlotTypes)))
        {
            if (i < 0)
                continue;

            var _indicator = Instantiate(m_slotIndicatorPrefab.gameObject, m_slotIndicatorsGroup).GetComponent<Image>();
            m_slotIndicatorImages.Add(_indicator);
        }

        m_previousTabInputRef.action.performed += this.onPreviousTabInput;
        m_nextTabInputRef.action.performed += this.onNextTabInput;

        if (m_previousTabInputRef.action.enabled == false)
            m_previousTabInputRef.action.Enable();

        if (m_nextTabInputRef.action.enabled == false)
            m_nextTabInputRef.action.Enable();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_previousTabInputRef.action.performed -= this.onPreviousTabInput;
        m_nextTabInputRef.action.performed -= this.onNextTabInput;
    }

    public void SetSlot(EquipmentSlotTypes slotType)
    {
        m_slotType = slotType;
    }

    protected override void onOpened()
    {
        base.onOpened();

        rebuild();
    }

    protected override void onCancelInput(InputAction.CallbackContext context)
    {
        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        DevelopmentScreen.Instance.OpenTab(1);
    }

    private void rebuild()
    {
        for (int i = m_activeCategoryGroups.Count; i-- > 0;)
            m_activeCategoryGroups[i].ResetAndReturnToPool();

        for (int i = m_activeDividers.Count; i-- > 0;)
            m_activeDividers[i].ResetAndReturnToPool();

        for (int i = 0; i < m_slotIndicatorImages.Count; i++)
            m_slotIndicatorImages[i].color = m_slotIndicatorColor_Inactive;

        m_activeCategoryGroups.Clear();
        m_activeDividers.Clear();
        m_categories.Clear();

        m_slotIndicatorImages[(int)m_slotType].color = m_slotIndicatorColor_Active;

        var _slotTypeString = Regex.Replace(m_slotType.ToString(), @"([a-z])([A-Z])", "$1 $2");
        m_slotNameText.SetText(_slotTypeString);

        var _buildParams = getBuildParams();

        m_currentItemDataPanel.Clear();
        m_selectedItemDataPanel.Clear();

        if (_buildParams.ValidEquipmentType == null)
            return;

        populateDataFromSaveFile();

        for (int i = 0; i < m_editEquipmentCollection.EquipmentAssets.Count; i++)
        {
            var _asset = m_editEquipmentCollection.EquipmentAssets[i];
            var _assetType = _asset.GetType();

            if (_assetType == _buildParams.ValidEquipmentType || _assetType.IsSubclassOf(_buildParams.ValidEquipmentType))
            {
                if (m_categories.Contains(_asset.Category) == false)
                    m_categories.Add(_asset.Category);
            }
        }

        var _currentlyEquippedAsset = m_editLoadout.Dictionary[m_slotType];

        if (_buildParams.CanUnequip)
        {
            var _emptyGroup = EquipmentCategoryGroupPool.Get();
            _emptyGroup.transform.SetParent(m_verticalGroupContent);

            _emptyGroup.Populate(
                category: null, 
                equipmentCollection: m_editEquipmentCollection,
                currentlyEquippedAsset: _currentlyEquippedAsset,
                onEquipmentSelectedCallback: onEquipmentSelected,
                onEquipmentClickedCallback: onEquipmentClicked);

            _emptyGroup.gameObject.SetActiveOptimized(true);
            m_activeCategoryGroups.Add(_emptyGroup);

            var _divider = HorizontalDividerPool.Get();
            _divider.transform.SetParent(m_verticalGroupContent);
            _divider.SetPreferredHeight(20);
            _divider.gameObject.SetActiveOptimized(true);
            m_activeDividers.Add(_divider);
        }

        for (int i = 0; i < m_categories.Count; i++)
        {
            var _category = m_categories[i];

            var _group = EquipmentCategoryGroupPool.Get();
            _group.transform.SetParent(m_verticalGroupContent);

            _group.Populate(
                category: _category,
                equipmentCollection: m_editEquipmentCollection,
                currentlyEquippedAsset: _currentlyEquippedAsset,
                onEquipmentSelectedCallback: onEquipmentSelected,
                onEquipmentClickedCallback: onEquipmentClicked);

            _group.gameObject.SetActiveOptimized(true);
            m_activeCategoryGroups.Add(_group);

            if (i < m_categories.Count-1)
            {
                var _divider = HorizontalDividerPool.Get();
                _divider.transform.SetParent(m_verticalGroupContent);
                _divider.SetPreferredHeight(20);
                _divider.gameObject.SetActiveOptimized(true);
                m_activeDividers.Add(_divider);
            }
        }

        if (_currentlyEquippedAsset != null)
            _currentlyEquippedAsset.PopulateDataPanel(m_currentItemDataPanel);
        else
            m_currentItemDataPanel.PopulateWithEmptyData();
    }

    private void populateDataFromSaveFile()
    {
        var _saveData = SaveManager.Instance.CurrentSave;

        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutListSerialized);
        _saveData.ReadObject(SaveIDConstants.UNLOCKED_EQUIPMENT_ID, ref m_unlockedEquipmentSerialized);

        m_editLoadout.PopulateFromSerializedData(m_loadoutListSerialized.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex]);
        m_editEquipmentCollection.PopulateFromSerializedData(m_unlockedEquipmentSerialized);
    }

    private struct BuildParams
    {
        public Type ValidEquipmentType;
        public bool CanUnequip;
    }

    private BuildParams getBuildParams()
    {
        var _result = new BuildParams
        {
            ValidEquipmentType = null,
            CanUnequip = false,
        };

        switch (m_slotType)
        {
            case EquipmentSlotTypes.Undefined:
                break;

            case EquipmentSlotTypes.LeftArm:
            case EquipmentSlotTypes.RightArm:
            case EquipmentSlotTypes.LeftShoulder:
            case EquipmentSlotTypes.RightShoulder:
                _result.ValidEquipmentType = typeof(PrimaryEquipment);
                _result.CanUnequip = true;
                break;

            case EquipmentSlotTypes.Melee:
                _result.CanUnequip = true;
                break;

            case EquipmentSlotTypes.Generator:
                break;

            case EquipmentSlotTypes.Head:
                _result.ValidEquipmentType = typeof(HeadEquipment);
                break;

            case EquipmentSlotTypes.Arms:
                _result.ValidEquipmentType = typeof(ArmsEquipment);
                break;

            case EquipmentSlotTypes.Body:
                _result.ValidEquipmentType = typeof(BodyEquipment);
                break;

            case EquipmentSlotTypes.Legs:
                _result.ValidEquipmentType = typeof(LegsEquipment);
                break;

            case EquipmentSlotTypes.Utility1:
            case EquipmentSlotTypes.Utility2:
            case EquipmentSlotTypes.Utility3:
            case EquipmentSlotTypes.Utility4:
                _result.CanUnequip = true;
                break;

            case EquipmentSlotTypes.Passive1:
            case EquipmentSlotTypes.Passive2:
            case EquipmentSlotTypes.Passive3:
            case EquipmentSlotTypes.Passive4:
                _result.CanUnequip = true;
                break;
        }

        return _result;
    }

    public void Button_SwitchSlot(int direction)
    {
        int _enumIndex = (int)m_slotType;
        _enumIndex += direction;

        if (_enumIndex < 0)
            _enumIndex += m_slotIndicatorImages.Count;
        else if (_enumIndex >= m_slotIndicatorImages.Count)
            _enumIndex = 0;

        m_slotType = (EquipmentSlotTypes)_enumIndex;

        rebuild();
    }

    protected override void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        base.onActiveInputDeviceChanged(deviceType);

        if (IsOpened == false || m_activeCategoryGroups.Count == 0)
            return;

        if (deviceType == InputDeviceTypes.KeyboardAndMouse)
            return;

        rebuild();
    }

    private void onPreviousTabInput(InputAction.CallbackContext context)
    {
        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        Button_SwitchSlot(-1);
    }

    private void onNextTabInput(InputAction.CallbackContext context)
    {
        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        Button_SwitchSlot(1);
    }

    private void onEquipmentSelected(Equipment equipment)
    {
        m_selectedItemDataPanel.Clear();

        if (equipment != null)
            equipment.PopulateDataPanel(m_selectedItemDataPanel);
        else
            m_selectedItemDataPanel.PopulateWithEmptyData();
    }

    private void onEquipmentClicked(Equipment equipment)
    {
        m_editLoadout.Dictionary[m_slotType] = equipment;
        m_loadoutListSerialized.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex] = m_editLoadout.Serialize();

        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.RegisterVariable(SaveIDConstants.LOADOUT_LIST_ID, m_loadoutListSerialized);

        SaveManager.Instance.SaveData();

        rebuild();
    }
}
