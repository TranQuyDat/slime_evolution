using UnityEngine;

abstract class  BaseVfxEvent : ScriptableObject
{
    public abstract void Play(VFXContext ctx = default);
}