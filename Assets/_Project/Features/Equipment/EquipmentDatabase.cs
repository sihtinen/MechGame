using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "MechGame/Equipment/New Equipment Database")]
public class EquipmentDatabase : ScriptableObject
{
    [SerializeField] private List<Equipment> m_allAssets = new List<Equipment>();

#if UNITY_EDITOR
    [ContextMenu("Rebuild Database")]
    private void Editor_RebuildDatabase()
    {
        m_allAssets.Clear();

        var _assetGUIDs = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(Equipment)}");

        if (_assetGUIDs != null && _assetGUIDs.Length > 0)
        {
            foreach (var _guid in _assetGUIDs)
            {
                var _path = UnityEditor.AssetDatabase.GUIDToAssetPath(_guid);
                var _asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Equipment>(_path);
                m_allAssets.Add(_asset);
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
    }
#endif
}