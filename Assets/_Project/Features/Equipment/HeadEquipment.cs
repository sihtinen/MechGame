using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Head Equipment")]
public class HeadEquipment : Equipment
{
    [Header("Targeting Settings")]
    public MechTargeting.TargetingSettings TargetingSettings = new();

    public override void InitializeGameplay(EquipmentRuntimeSetupData setupData)
    {
        setupData.Mech.TargetingComponent.Settings = TargetingSettings;
    }
}
