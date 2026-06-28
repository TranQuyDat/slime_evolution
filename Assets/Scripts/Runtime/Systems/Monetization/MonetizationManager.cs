using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;

class MonetizationManager : MonoBehaviour
{
    public static MonetizationManager Instance;
    [Header("Setting Sdk")]
    [SerializeField]private AdProviBase _AdProviderOnly;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        InitalizeSdk();
    }

    private void InitalizeSdk()
    {
        if(_AdProviderOnly == null) return;
        _AdProviderOnly.Initialize(() =>
        {
            Debug.Log($"{_AdProviderOnly.GetType().Name} is ready.");
        });
    }

    public void ShowAd(Action OnComplete = null)
    {
        _AdProviderOnly?.ShowRewarded(OnComplete);
    }
    public async void HideBannerAd(Button btnHide)
    {
        _AdProviderOnly?.HideBanner();
        btnHide.interactable = false;
        await Task.Delay(60000);
        ShowBannerAd();
        btnHide.interactable = true;
    }
    public void ShowBannerAd()
    {
        _AdProviderOnly?.ShowBanner();
        _AdProviderOnly.RefreshBanner();
    }
}