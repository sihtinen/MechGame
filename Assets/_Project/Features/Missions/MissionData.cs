using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/Missions/New Mission Data")]
public class MissionData : ScriptableObject
{
    public string DisplayName = "M00 - Mission Name";
    [TextArea] public string Description = "Describe the mission here";
    public SceneReference Scene;
}