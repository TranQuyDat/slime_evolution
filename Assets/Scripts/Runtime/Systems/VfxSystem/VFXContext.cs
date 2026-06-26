using UnityEngine;

public struct VFXContext
{
    public Vector3 Position;
    public Transform Parent;
    public Vector3 Scale;
    public float Speed;
    public static readonly VFXContext Default = new()
    {
        Scale = Vector3.one,
        Speed = 1f
    };
}