using UnityEngine;

abstract class BaseAudioEvent : ScriptableObject
{
    public abstract void Play(AudioContext ctx = default);
}

