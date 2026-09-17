using System;
using UnityEngine;

public abstract class AdProviderBase : ScriptableObject
{
    public abstract bool IsReady();
    public abstract void Initialize(Action onComplete);
    public abstract void ShowAd(Action onComplete = null);
}
