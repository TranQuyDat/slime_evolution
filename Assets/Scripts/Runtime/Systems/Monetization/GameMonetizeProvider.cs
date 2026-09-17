using System;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GameMonetizeProvider",
    menuName = "Ads/GameMonetizeProvider")]
class GameMonetizeProvider : AdProviderBase
{
    [SerializeField] private string _gameId = "YOUR_GAME_ID_HERE";

    private Action _pendingAdCallback;
    private GameMonetizeCallbackReceiver _receiver;
    private bool _isReady;

    public override bool IsReady() => _isReady;

    public override void Initialize(Action onInitComplete)
    {
#if UNITY_WEBGL || UNITY_EDITOR
        CreateCallbackReceiver();
        InitApi(_gameId);
        _isReady = true;
#else
        _isReady = true;
        onInitComplete?.Invoke();
#endif
    }

    public override void ShowAd(Action onComplete = null)
    {
        Debug.Log($"[GameMonetizeProvider] Enter ShowAd. IsReady: {IsReady()}");

        if (!IsReady())
        {
            Debug.LogWarning(
                "[GameMonetizeProvider] ShowAd skipped: provider is not ready.");
            onComplete?.Invoke();
            return;
        }

        _pendingAdCallback = onComplete;
#if UNITY_WEBGL || UNITY_EDITOR
        Debug.Log("[GameMonetizeProvider] Ad request sent to GameMonetize SDK.");
        ShowGameMonetizeAd(_gameId);
#else
        Debug.Log(
            "[GameMonetizeProvider] Editor simulation: ad completed.");
        HandleAdResumed();
#endif
    }

    private void CreateCallbackReceiver()
    {
        GameObject receiverObject = GameObject.Find("GameMonetize");
        if (receiverObject == null)
        {
            receiverObject = new GameObject("GameMonetize");
            DontDestroyOnLoad(receiverObject);
        }

        _receiver = receiverObject.GetComponent<GameMonetizeCallbackReceiver>();
        if (_receiver == null)
            _receiver = receiverObject.AddComponent<GameMonetizeCallbackReceiver>();

        _receiver.Initialize(HandleAdPaused, HandleAdResumed);
    }

    private static void HandleAdPaused()
    {
        AudioListener.pause = true;
    }

    private void HandleAdResumed()
    {
        AudioListener.pause = false;

        Action callback = _pendingAdCallback;
        _pendingAdCallback = null;
        callback?.Invoke();
    }

#if UNITY_WEBGL || UNITY_EDITOR
    [DllImport("__Internal", EntryPoint = "InitApi")]
    private static extern void InitApi(string gameId);

    [DllImport("__Internal", EntryPoint = "ShowBanner")]
    private static extern void ShowGameMonetizeAd(string gameId);
#endif
}
