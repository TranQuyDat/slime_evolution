using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;

class MonetizationManager : MonoBehaviour
{
    public static MonetizationManager Instance;
    [Header("Setting Sdk")]
    [SerializeField]private AdProviBase _AdProvider;
    [SerializeField]private Button _btnHideBanners;
    [SerializeField]private HudManager _hud;

    private bool _isWaittingToshowbtnHide;
    private SafeDelay _saveDelay = new SafeDelay();
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        _hud = GameManager.Instance.Hud;
    }
    void Start()
    {
        InitalizeSdk();
        _hud.OnChangeHud += HanldechangeHud;
    }

    private void InitalizeSdk()
    {
        if(_AdProvider == null) return;
        _AdProvider.Initialize(() =>
        {
            Debug.Log($"{_AdProvider.GetType().Name} is ready.");
            ShowBannerAd();

        });
    }

    private void HanldechangeHud(StateType type)
    {
        if(_AdProvider == null) return;
        if(type == StateType.Play || type == StateType.Pause)
        {
            HideBannerAd();
            return;
        } 
        if(!_AdProvider.IsBannerInvisible) return;
        ShowBannerAd();
    }

    public void ShowAdReward(Action OnComplete)
    {
        _AdProvider?.ShowRewarded(OnComplete);
    }
    public void ShowAdMidGame(Action OnComplete = null)
    {
        _AdProvider?.ShowAdMidGame(OnComplete);
    }

    public void HideBannerAd()
    {
        _AdProvider?.HideBanner();
    }
    public async void ShowBannerAd()
    {
        _AdProvider?.ShowBanner();
    }
}