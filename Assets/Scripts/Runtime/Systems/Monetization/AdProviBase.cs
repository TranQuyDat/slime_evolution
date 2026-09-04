using System;
using UnityEngine;

public abstract class AdProviBase: ScriptableObject
{
    public bool IsBannerInvisible { get; protected set; }
    public abstract bool IsReady();
    public abstract void Initialize(Action onInitComplete);
    public abstract void ShowBanner();
    public abstract void HideBanner();
    public abstract void ShowRewarded(Action onRewardSuccess);
    public abstract void ShowAdMidGame(Action onSuccess);
}
