using System;
using CrazyGames;
using UnityEngine;
[CreateAssetMenu(fileName ="CrazyGamesProvider",menuName ="Ads/CrazyGamesProvider")]
class CrazyGamesProvider : AdProviBase
{
    private bool _isInitialized = false;
    public bool IsReady()
    {
        return _isInitialized && CrazySDK.IsInitialized;
    }

    public override void Initialize(Action onInitComplete)
    {
        CrazySDK.Init(() =>
        {
            _isInitialized = true;
            CrazySDK.Banner.RefreshBanners();

            if(onInitComplete !=null)
                onInitComplete.Invoke();
        });
    }

    public override void HideBanner()
    {
        CrazySDK.Banner.Banners.ForEach(b => b.gameObject.SetActive(false));
    }

    public override void ShowBanner()
    {
        CrazySDK.Banner.Banners.ForEach(b => b.gameObject.SetActive(true));
    }

    public override void RefreshBanner()
    {
        CrazySDK.Banner.RefreshBanners();
    }

    public override void ShowRewarded(Action onRewardSuccess = null)
    {
        if(!IsReady()) return;
        CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded,default,
        adError:(error)=>
        {
            Debug.LogError(error);
        },
        adFinished:onRewardSuccess
        );
    }
}