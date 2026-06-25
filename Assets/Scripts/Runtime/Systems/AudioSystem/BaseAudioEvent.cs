using UnityEngine;

abstract class BaseAudioEvent : ScriptableObject
{
    public abstract void play(AudioSource source,AudioContext? ctx = null);
}

