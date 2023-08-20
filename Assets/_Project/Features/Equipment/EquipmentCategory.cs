using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Category")]
public class EquipmentCategory : ScriptableObject
{
    public GUIDWrapper GUID = new GUIDWrapper();

    public string DisplayName = "Category";
}