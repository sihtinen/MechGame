using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Camera/New Camera Rig Pose")]
public class MechCameraRigPose : ScriptableObject
{
    public float FieldOfView;
    public float FieldOfViewUpdateSpeed;
    [Space]
    public float FollowDistance;
    public float FollowDistanceUpdateSpeed;
    [Space]
    public float FollowHeightOffset;
    public float FollowHeightOffsetUpdateSpeed;
    [Space]
    public Vector3 VelocityOffsetFactor;
    public Vector3 VelocityOffsetMin;
    public Vector3 VelocityOffsetMax;
    public float VelocityOffsetSmoothTime;
    public float VelocityOffsetMaxSpeed;
}
