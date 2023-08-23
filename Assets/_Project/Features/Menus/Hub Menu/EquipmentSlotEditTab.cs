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
        base.onCancelInput(context);

        if (IsOpened == false)
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

        Type _validEquipmentType = getValidEquipmentType();

        if (_validEquipmentType == null)
            return;

        populateDataFromSaveFile();

        for (int i = 0; i < m_editEquipmentCollection.EquipmentAssets.Count; i++)
        {
            var _asset = m_editEquipmentCollection.EquipmentAssets[i];
            var _assetType = _asset.GetType();

            if (_assetType == _validEquipmentType || _assetType.IsSubclassOf(_validEquipmentType))
            {
                if (m_categories.Contains(_asset.Category) == false)
                    m_categories.Add(_asset.Category);
            }
        }

        for (int i = 0; i < m_categories.Count; i++)
        {
            var _category = m_categories[i];

            var _group = EquipmentCategoryGroupPool.Get();
            _group.transform.SetParent(m_verticalGroupContent);
            _group.Populate(_category, m_editEquipmentCollection);
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
    }

    private void populateDataFromSaveFile()
    {
        var _saveData = SaveManager.Instance.CurrentSave;

        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutListSerialized);
        _saveData.ReadObject(SaveIDConstants.UNLOCKED_EQUIPMENT_ID, ref m_unlockedEquipmentSerialized);

        m_editLoadout.PopulateFromSerializedData(m_loadoutListSerialized.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex]);
        m_editEquipmentCollection.PopulateFromSerializedData(m_unlockedEquipmentSerialized);
    }

    private Type getValidEquipmentType()
    {
        Type _validEquipmentType = null;

        switch (m_slotType)
        {
            case EquipmentSlotTypes.Undefined:
                break;

            case EquipmentSlotTypes.LeftArm:
            case EquipmentSlotTypes.RightArm:
            case EquipmentSlotTypes.LeftShoulder:
            case EquipmentSlotTypes.RightShoulder:
                _validEquipmentType = typeof(PrimaryEquipment);
                break;

            case EquipmentSlotTypes.Melee:
                break;

            case EquipmentSlotTypes.Generator:
                break;

            case EquipmentSlotTypes.Head:
                _validEquipmentType = typeof(HeadEquipment);
                break;

            case EquipmentSlotTypes.Arms:
                _validEquipmentType = typeof(ArmsEquipment);
                break;

            case EquipmentSlotTypes.Body:
                _validEquipmentType = typeof(BodyEquipment);
                break;

            case EquipmentSlotTypes.Legs:
                _validEquipmentType = typeof(LegsEquipment);
                break;

            case EquipmentSlotTypes.Utility1:
            case EquipmentSlotTypes.Utility2:
            case EquipmentSlotTypes.Utility3:
            case EquipmentSlotTypes.Utility4:

                break;

            case EquipmentSlotTypes.Passive1:
            case EquipmentSlotTypes.Passive2:
            case EquipmentSlotTypes.Passive3:
            case EquipmentSlotTypes.Passive4:
                break;
        }

        return _validEquipmentType;
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
}
