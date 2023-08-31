using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Thruster Equipment")]
public class ThrusterEquipment : PrimaryEquipment
{
    [Min(0f)] public float RechargeRate = 1.0f;

    [Header("Normal Thrust")]
    public bool EnableThrust_Normal = true;
    [Min(0f)] public float HorizontalForce_Normal;
    [Min(0f)] public float VerticalForce_Normal;
    [Min(0f)] public float EnergyDrainRate_Normal = 1f;

    [Header("Boost Thrust")]
    public bool EnableThrust_Boost = true;
    [Min(0f)] public float HorizontalForce_Boost;
    [Min(0f)] public float VerticalForce_Boost;
    [Min(0f)] public float EnergyDrainRate_Boost = 1f;

    [Header("HUD Settings")]
    public HUDThrusterEquipmentElement HUDPrefab = null;

    public override void InitializeGameplay(EquipmentRuntimeSetupData setupData)
    {
        var _thrusterComp = initializeRuntimeComponent<MechThrusterRuntime>(setupData);

        if (setupData.IsPlayer == false)
            return;

        _thrusterComp.BindToInput(setupData.InputActionRef);

        var _hudElement = HUDManager.Instance.InitializeHUDElement(HUDPrefab, setupData.SlotType);
        _hudElement.BindToRuntimeComponent(_thrusterComp);
    }

    protected override void populateDataPanel_Custom(DataPanel dataPanel)
    {
        dataPanel.CreateTextElement().Initialize("Recharge Rate", RechargeRate.ToString("0"), 18, 18);
    }
}
