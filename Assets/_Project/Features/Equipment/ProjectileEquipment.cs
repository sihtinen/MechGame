using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Projectile Equipment")]
public class ProjectileEquipment : PrimaryEquipment
{
    [Header("Projectile Settings")]
    [Min(1)] public int UseCount = 320;
    [Min(0)] public float UseRatePerSecond = 1;
    public ContextTargetLayers TargetLayers = ContextTargetLayers.None;
    [Min(0)] public int Damage = 1;
    [Min(0)] public float CollisionRadius = 1;
    [Min(0)] public float Lifetime = 10;
    [Min(0)] public float Velocity = 20f;

    [Header("UI Settings")]
    public HUDEquipmentElementBase HUDPrefab = null;

    public override void InitializeGameplay(EquipmentRuntimeSetupData setupData)
    {
        var _projectileRuntimeComponent = initializeRuntimeComponent<MechProjectileRuntime>(setupData);

        if (setupData.IsPlayer == false)
            return;

        _projectileRuntimeComponent.BindToInput(setupData.InputActionRef);

        var _hudElement = HUDManager.Instance.InitializeHUDElement(HUDPrefab, setupData.SlotType);
        _hudElement.BindToRuntimeComponent(_projectileRuntimeComponent);
    }

    protected override void populateDataPanel_Custom(DataPanel dataPanel)
    {
        dataPanel.CreateTextElement().Initialize("Damage", Damage.ToStringMinimalAlloc());
        dataPanel.CreateTextElement().Initialize("Capacity", UseCount.ToStringMinimalAlloc());
        dataPanel.CreateTextElement().Initialize("Rate of fire", UseRatePerSecond.ToString("0.0") + "/s");
        dataPanel.CreateTextElement().Initialize("Projectile speed", Velocity.ToString("0"));
    }
}
