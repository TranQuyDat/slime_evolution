using System;
using UnityEngine;

public abstract class AdProviBase: ScriptableObject
{
    public abstract void Initialize(Action onInitComplete);
    public abstract void ShowBanner();
    public abstract void HideBanner();
    public abstract void RefreshBanner();
    public abstract void ShowRewarded(Action onRewardSuccess);
}