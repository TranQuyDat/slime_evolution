using System;
using UnityEngine;

[Serializable]
public struct VFXContext
{
    public bool OverrideDuration;
    public bool OverridePosition;
    public bool OverrideScale;
    public bool OverrideSpeed;
    public bool OverrideColor;
    public bool OverrideParent;

    public float Duration;
    public Vector3 Position;
    public Transform Parent;
    public Vector3 Scale;
    public float Speed;
    public Color Color;

    public static readonly VFXContext Default = new()
    {
        Scale = Vector3.one,
        Speed = 1f,
        OverrideScale = true,
        OverrideSpeed = true
    };

    public VFXContext Merge(VFXContext overrideCtx)
    {
        var result = this;

        if (overrideCtx.Duration != default) result.Duration = overrideCtx.Duration;
        if (overrideCtx.Position != default) result.Position = overrideCtx.Position;
        if (overrideCtx.Scale != default) result.Scale = overrideCtx.Scale;
        if (overrideCtx.Speed != default) result.Speed = overrideCtx.Speed;
        if (overrideCtx.Color != default) result.Color = overrideCtx.Color;
        if (overrideCtx.Parent != default) result.Parent = overrideCtx.Parent;

        return result;
    }
}