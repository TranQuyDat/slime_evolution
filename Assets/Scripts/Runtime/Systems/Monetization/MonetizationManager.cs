using System;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(-2)]
class MonetizationManager : MonoBehaviour
{
    public static MonetizationManager Instance { get; private set; }

    [FormerlySerializedAs("_AdProvider")]
    [SerializeField] private AdProviderBase _adProvider;
    [SerializeField] private HudManager _hud;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _adProvider?.Initialize(() =>
            Debug.Log($"{_adProvider.GetType().Name} initialized."));
    }

    private void Start()
    {
        if (_hud == null)
            _hud = GameManager.Instance.Hud;

        _hud.OnChangeHud += HandleChangeHud;
    }

    private void OnDestroy()
    {
        if (_hud != null)
            _hud.OnChangeHud -= HandleChangeHud;

        if (Instance == this)
            Instance = null;
    }

    private void HandleChangeHud(StateType type)
    {
        if (type == StateType.Play)
            ShowAd();
    }

    public void ShowAd(Action onComplete = null)
    {
        if (_adProvider == null)
        {
            onComplete?.Invoke();
            return;
        }

        _adProvider.ShowAd(onComplete);
    }
}
