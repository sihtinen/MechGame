using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/AI/New Context Processor Settings")]
public class ContextProcessorSettings : ScriptableObject
{
    [Min(1)] public int ResultInterpolationSteps = 4;
    public float ResultSmoothTime = 1f;

    public float CollisionSphereRadius = 10f;
    public List<ContextSampleLayer> SampleLayers = new List<ContextSampleLayer>();

    [System.Serializable]
    public class ContextSampleLayer
    {
        [Min(1)] public int SampleCount = 8;
        public float CollisionDistance = 50;
        public float HeightOffset = 0f;
    }
}
