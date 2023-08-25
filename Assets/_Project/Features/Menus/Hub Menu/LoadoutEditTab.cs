using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutEditTab : UITab
{
    [SerializeField] private RectTransform m_scrollContentRoot = null;
    [SerializeField] private MechLoadout m_selectedLoadoutAsset = null;

    private MechLoadout.MechLoadoutListSerialized m_loadoutList = new();

    protected override void onOpened()
    {
        base.onOpened();

        rebuild();

        onActiveInputDeviceChanged(getActiveInputDevice());
    }

    protected override void onActiveInputDeviceChanged(InputDeviceTypes deviceType)
    {
        if (IsOpened == false || gameObject.activeInHierarchy == false)
            return;

        rebuild();

        if (deviceType != InputDeviceTypes.KeyboardAndMouse)
            setFirstActiveChildAsSelected(m_scrollContentRoot);
    }

    private void rebuild()
    {

        m_loadoutList.AllLoadouts.Clear();

        var _saveData = SaveManager.Instance.CurrentSave;
        _saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_loadoutList);

        var _loadoutDataSerialized = m_loadoutList.AllLoadouts[ManageLoadoutsTab.EditLoadoutIndex];
        m_selectedLoadoutAsset.PopulateFromSerializedData(_loadoutDataSerialized);
    }
}