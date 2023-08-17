using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutTab : UITab
{
    [Header("Object References")]
    [SerializeField] private RectTransform m_loadoutButtonGroupTransform = null;
    [SerializeField] private AnimatedButton m_createNewLoadoutButton = null;
    [SerializeField] private MechLoadout m_selectedLoadoutAsset = null;

    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new MechLoadout.MechLoadoutListSerialized();

    protected override void onOpened()
    {
        base.onOpened();

        rebuild();
    }

    private void rebuild()
    {
        LoadoutButtonPool.ResetUsedObjects();

        var _saveData = SaveManager.Instance.CurrentSave;

        var _selectedLoadoutIndex = _saveData.ReadInt(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID).Item2;

        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        for (int i = 0; i < m_loadoutList.AllLoadouts.Count; i++)
        {
            var _loadoutSerialized = m_loadoutList.AllLoadouts[i];
            m_selectedLoadoutAsset.PopulateFromSerializedData(_loadoutSerialized);

            var _newLoadoutButton = LoadoutButtonPool.Get();

            _newLoadoutButton.Populate(
                mechLoadout: m_selectedLoadoutAsset, 
                isSelected: i == _selectedLoadoutIndex);

            _newLoadoutButton.transform.SetAsFirstSibling();
            _newLoadoutButton.gameObject.SetActiveOptimized(true);
        }

        if (m_loadoutList.AllLoadouts.Count < 8)
        {
            m_createNewLoadoutButton.transform.SetAsLastSibling();
            m_createNewLoadoutButton.gameObject.SetActiveOptimized(true);
        }
        else
            m_createNewLoadoutButton.gameObject.SetActiveOptimized(false);
    }

    public void Button_LoadoutSelected(int loadoutIndex)
    {

    }

    public void Button_CreateNewLoadout()
    {

    }
}