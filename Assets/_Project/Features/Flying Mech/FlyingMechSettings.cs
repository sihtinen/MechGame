using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Flying Mechs/New Flying Mech Settings")]
public class FlyingMechSettings : ScriptableObject
{
    [Min(1)] public int Health = 750;

    [Space]

    public float GravityMultiplier = 10f;

    public Vector2 FlyHeightNoiseSpeed = Vector2.zero;
    [Min(0f)] public float FlyHeightNoiseStrength = 0f;

    public float ForwardMoveSpeed = 20f;
    public float SidewaysMoveSpeed = 20f;

    [Header("Debug")]
    public bool DrawDebugLines = false;
    public Color DebugColor_ToMoveTarget = Color.yellow;
}
