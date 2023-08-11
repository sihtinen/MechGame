using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Mechs/New Mech Settings")]
public class MechSettings : ScriptableObject
{
    [Min(1)] public int Health = 1000;

    [Space]

    public float MaxMovementSpeed = 20f;
    public AnimationCurve MovementForwardDirectionMultCurve = new AnimationCurve();

    [Space]

    public float MinTiltDot = 0.55f;
    public float ForwardMovementTiltForceMult = 0.2f;
    public float SidewaysMovementTiltForceMult = 0.2f;

    [Space]

    public float HorizontalDrag_Ground = 2f;
    public float HorizontalDrag_Air = 1.6f;
    public float GravityMultiplier = 3.0f;

    [Header("Ground Check")]
    [Min(0f)] public float GroundRideHeight = 10f;
    public float GroundCheckHeight = 10f;
    [Min(0f)] public float GroundCheckDistance = 15f;
    [Min(0f)] public float GroundCheckRadius = 10f;
    public LayerMask GroundCheckLayers;
}
