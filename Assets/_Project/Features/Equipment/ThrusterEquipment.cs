using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Thruster Equipment")]
public class ThrusterEquipment : PrimaryEquipment
{
    [Min(0f)] public float RechargeRate = 1.0f;

    [Header("Normal Boost")]
    public BoostParameters BoostParams_Horizontal = new();
    public BoostParameters BoostParams_Vertical = new();
    [Min(0f)] public float EnergyDrainRate_Normal = 1f;

    [Header("Dash Booost")]
    public BoostParameters DashBoostParams_Horizontal = new();
    public BoostParameters DashBoostParams_Vertical = new();
    [Min(0f)] public float DashEnergyDrain = 20;
    [Min(0f)] public float DashPrepareDuration = 0.4f;
    [Min(0f)] public float DashBoostDuration = 1.4f;
    [Range(0f, 1f)] public float DashVerticalVelocityCancelAmount = 0.5f; 
    public AnimationCurve DashBoostForceCurve = new();

    [System.Serializable]
    public struct BoostParameters
    {
        public float HorizontalForce;
        public float VerticalForce;

        public static BoostParameters Lerp(BoostParameters a, BoostParameters b, float t)
        {
            return new BoostParameters
            {
                HorizontalForce = Mathf.Lerp(a.HorizontalForce, b.HorizontalForce, t),
                VerticalForce = Mathf.Lerp(a.VerticalForce, b.VerticalForce, t),
            };
        }
    }

    [Header("HUD Settings")]
    public HUDThrusterEquipmentElement HUDPrefab = null;

    public BoostParameters GetBoostParams(float inputAmount)
    {
        return BoostParameters.Lerp(BoostParams_Vertical, BoostParams_Horizontal, inputAmount);
    }

    public BoostParameters GetDashBoostParams(float inputAmount)
    {
        return BoostParameters.Lerp(DashBoostParams_Vertical, DashBoostParams_Horizontal, inputAmount);
    }

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
