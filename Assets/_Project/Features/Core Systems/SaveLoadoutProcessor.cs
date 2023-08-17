using System.Collections;
using System.Collections.Generic;
using Tensori.SaveSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Save Data/New Loadout Processor")]
public class SaveLoadoutProcessor : SavePreProcessor
{
    [SerializeField] private MechLoadout m_defaultLoadoutTemplate = null;

    private static MechLoadout.MechLoadoutListSerialized m_cachedLoadoutList = null;

    public override void PreProcess(SaveData saveData)
    {
        if (m_cachedLoadoutList == null)
            m_cachedLoadoutList = new MechLoadout.MechLoadoutListSerialized();
        else
            m_cachedLoadoutList.AllLoadouts.Clear();

        bool _listFound = saveData.ReadObject(SaveIDConstants.LOADOUT_LIST_ID, ref m_cachedLoadoutList);

        if (_listFound == false)
            saveData.RegisterVariable(SaveIDConstants.LOADOUT_LIST_ID, m_cachedLoadoutList);

        if (m_cachedLoadoutList.AllLoadouts.Count < 1)
        {
            m_cachedLoadoutList.AllLoadouts.Add(m_defaultLoadoutTemplate.Serialize());
            saveData.RegisterVariable(SaveIDConstants.LOADOUT_LIST_ID, m_cachedLoadoutList);
            saveData.RegisterVariable<int>(SaveIDConstants.ACTIVE_LOADOUT_INDEX_ID, 0);
        }
    }
}