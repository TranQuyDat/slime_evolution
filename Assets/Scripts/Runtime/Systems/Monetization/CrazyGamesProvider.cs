using System;
using System.Threading.Tasks;
using CrazyGames;
using UnityEngine;
[CreateAssetMenu(fileName ="CrazyGamesProvider",menuName ="Ads/CrazyGamesProvider")]
class CrazyGamesProvider : AdProviBase
{
    public bool IsReady()
    {
        return CrazySDK.IsAvailable && CrazySDK.IsInitialized;
    }

    public override void Initialize(Action onInitComplete)
    {
        if(!CrazySDK.IsAvailable) return;
        CrazySDK.Init(() =>
        {
            if(onInitComplete !=null)
                onInitComplete.Invoke();
        });
    }

    public override void HideBanner()
    {
        if(!IsReady()) return;
        CrazySDK.Banner.Banners.ForEach(b => b.gameObject.SetActive(false));
        IsBannerInvisible = true;
        CrazySDK.Banner.RefreshBanners();
    }

    public override void ShowBanner()
    {
        if(!IsReady()) return;
        CrazySDK.Banner.Banners.ForEach(b => b.gameObject.SetActive(true));
        IsBannerInvisible = false;
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

    public override void ShowAdMidGame(Action onSuccess)
    {
        if(!IsReady()) return;
        CrazySDK.Ad.RequestAd(CrazyAdType.Midgame,default,
        adError:(error)=>
        {
            Debug.LogError(error);
        },
        adFinished:onSuccess
        );
    }
}