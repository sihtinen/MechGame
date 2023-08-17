using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDatabaseAccess : SingletonBehaviour<EquipmentDatabaseAccess>
{
    [Header("Object References")]
    [SerializeField] private EquipmentDatabase m_database = null;
    public EquipmentDatabase Database => m_database;
}
