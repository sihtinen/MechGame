using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct ContextData : IDisposable
{
    public bool IsCreated => Samplers.IsCreated;

    public NativeArray<ContextSampler> Samplers;

    public void Dispose()
    {
        if (Samplers.IsCreated)
            Samplers.Dispose();
    }
}

public struct ContextSampler
{
    public float3 PositionOffset;
    public float3 Direction;
    public float Distance;
    public float Value;
}
