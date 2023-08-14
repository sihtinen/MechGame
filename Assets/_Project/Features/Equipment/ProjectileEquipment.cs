using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Equipment/New Projectile Equipment")]
public class ProjectileEquipment : Equipment
{
    [Header("Projectile Settings")]
    [Min(1)] public int UseCount = 320;
    [Min(1)] public int UseRatePerSecond = 1;
    public ContextTargetLayers TargetLayers = ContextTargetLayers.None;
    [Min(0)] public int Damage = 1;
    [Min(0)] public float CollisionRadius = 1;
    [Min(0)] public float Lifetime = 10;
    [Min(0)] public float Velocity = 20f;

    [Header("Targeting Settings")]
    public float TargetingMinDot = 0.3f;
    [Range(0f, 1f)] public float TargetingCameraDotScore = 1f;
    [Space]
    [Range(0f, 1f)] public float TargetingComponentDotScore = 1f;
    [Space]
    public float TargetingDistance = 300f;
    public float TargetingOptimalDistance = 100f;
    [Range(0f, 1f)] public float TargetingDistanceScoreMultiplier = 1f;

    [Header("UI Settings")]
    public HUDEquipmentElementBase HUDPrefab = null;
    [Space]
    public Sprite HUD_ValidTarget = null;
    [Min(0f)] public float HUD_ValidTarget_Size = 25f;
    public Sprite HUD_ActiveTarget = null;
    [Min(0f)] public float HUD_ActiveTarget_Size = 25f;
    public Sprite HUD_Prediction = null;
    [Min(0f)] public float HUD_Prediction_Size = 25f;

    public override void InitializeGameplay(MechController mech, EquipmentSlot slot, bool isPlayer, InputActionReference inputActionRef)
    {
        var _projectileRuntimeComponent = initializeRuntimeComponent<MechProjectileRuntime>(mech, slot);

        if (isPlayer == false)
            return;

        _projectileRuntimeComponent.BindToInput(inputActionRef);

        var _hudElement = HUDManager.Instance.InitializeHUDElement(HUDPrefab, slot.SlotType);
        _hudElement.BindToRuntimeComponent(_projectileRuntimeComponent);

        TargetingHUDManager.Instance.RegisterProjectileRuntime(_projectileRuntimeComponent);
    }
}
