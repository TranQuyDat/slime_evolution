using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class PlayPanel : IState
{
    [SerializeField]private Button _btnPause;
    [SerializeField] private Button _btnMute;
    [SerializeField]private Button _btnTrigerRemove3SlimesSupport;
    [SerializeField]private Button _btnCancleRemoveSlime;
    [SerializeField]private Button _btnRemove3SlimesSupport;
    [SerializeField]private Image _ImgPreview;
    [SerializeField]private TextMeshProUGUI _txtScore;
    [SerializeField]private TextMeshProUGUI _txtCombo;
    [SerializeField]private FloatingScore _prefabFloatingScore;
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    [SerializeField]private TextMeshProUGUI _txtRemove3Slimes;
    [SerializeField]private float _scoreCountDuration = 0.5f;

    [SerializeField]private GameObject _PanelSelectSlimes;
    [SerializeField]private Sprite _spriteBG;

    private float _timeDelay;
    private int _displayedScore;
    private Tween _scoreTween;
    private Sequence _selectionSequence;
    private readonly PlayPanelTransition _transition = new PlayPanelTransition();
    
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _hud = GameManager.Instance.Hud;
        _hud.SetBackGround(_spriteBG);
    }
    void OnEnable()
    {
        _hud.OnChangeHud +=HandleChangeHud;
    }
    void Update()
    {
        if (_txtCombo.gameObject.activeSelf)
        {
            WaitToHideCombo(1.5f);
        }
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _txtCombo.gameObject.SetActive(false);
        _btnTrigerRemove3SlimesSupport.gameObject.SetActive(true);
        _PanelSelectSlimes.SetActive(false);

        _btnPause.onClick.AddListener(BtnPause);
        if (_btnMute != null)
        {
            _btnMute.onClick.AddListener(BtnMute);
            _hud.RefreshMuteButton(_btnMute);
        }
        _btnRemove3SlimesSupport.onClick.AddListener(BtnRemove3SlimesSupport);
        _btnTrigerRemove3SlimesSupport.onClick.AddListener(BtnTrigerRemove3SlimesSupport);
        _btnCancleRemoveSlime. onClick.AddListener(BtnCancleSelectSlimePanel);

        _hud.OnCommand +=HandleUpdateHightScore;
        _hud.OnCommand +=HandleUpdatePreview;
        _hud.OnCommand +=HandleUpdateScore;
        _hud.OnCommand +=HandleUpdateCombo;
        _hud.OnCommand +=HandleUpdateRemove3SlimesSupport;
        _hud.OnCommand +=HandleFloatingScore;
        _hud.OnCommand += HandleCancelSelectSlimePanel;
        _hud.OnCommand += HandleFlyPreviewToSpawn;

        _transition.PlayIntro(
            _txtScore.transform,
            _txtHightScore.transform,
            _btnPause.transform,
            _btnTrigerRemove3SlimesSupport.transform,
            _ImgPreview.transform,
            _btnMute != null ? _btnMute.transform : null);
    }

    public override void Exit()
    {
        _scoreTween?.Kill();
        _selectionSequence?.Kill();

        _hud.OnCommand -=HandleUpdateHightScore;
        _hud.OnCommand -=HandleUpdatePreview;
        _hud.OnCommand -=HandleUpdateScore;
        _hud.OnCommand -=HandleUpdateCombo;
        _hud.OnCommand -=HandleUpdateRemove3SlimesSupport;
        _hud.OnCommand -=HandleFloatingScore;
        _hud.OnCommand -= HandleCancelSelectSlimePanel;
        _hud.OnCommand -= HandleFlyPreviewToSpawn;

        _btnPause.onClick.RemoveAllListeners();
        if (_btnMute != null)
            _btnMute.onClick.RemoveListener(BtnMute);
        _btnRemove3SlimesSupport.onClick.RemoveAllListeners();
        _btnTrigerRemove3SlimesSupport.onClick.RemoveAllListeners();
        _btnCancleRemoveSlime.onClick.RemoveAllListeners();
    }
    private void HandleChangeHud(StateType type)
    {
        if(type != StateType.Menu) return;
        this.gameObject.SetActive(false);
        _hud.OnChangeHud -=HandleChangeHud;
        _txtCombo.gameObject.SetActive(false);
    }

    // buttons
    private void BtnPause() 
    { 
        DOTween.Sequence()
        .Append(_transition.FxClickBtn(_btnPause.transform,0.15f))
        .OnComplete(() =>
        {
            _hud.ChangeHud(StateType.Pause);
            _hud.SendCommand(CommandType.Pause);
        });
    }
    private void BtnMute()
    {
        _transition.FxClickBtn(_btnMute.transform, () =>
            _hud.ToggleMute(_btnMute));
    }
    private void BtnTrigerRemove3SlimesSupport()
    {
        DOTween.Sequence()
        .Append(_transition.FxClickBtn(_btnTrigerRemove3SlimesSupport.transform,0.15f))
        .OnComplete(() =>
        {
            _PanelSelectSlimes.SetActive(true);
            _btnTrigerRemove3SlimesSupport.gameObject.SetActive(false);
            _btnPause.interactable = false;

            Transform selectionTitle = FindSelectionTitle();
            _selectionSequence = _transition.FxShowSelection(
                selectionTitle,
                _txtRemove3Slimes.transform,
                _btnCancleRemoveSlime,
                _btnRemove3SlimesSupport);

            _hud.SendCommand(CommandType.TrigerRemove3Slimes);
        });
    }
    private void BtnRemove3SlimesSupport()
    {
        DOTween.Sequence()
        .Append(_transition.FxClickBtn(_btnRemove3SlimesSupport.transform,0.15f))
        .OnComplete(() =>
        {
            _hud.SendCommand(CommandType.Remove3Slimes);
        });
    }
    private void BtnCancleSelectSlimePanel()
    {
        DOTween.Sequence()
        .Append(_transition.FxClickBtn(_btnCancleRemoveSlime.transform,0.15f))
        .OnComplete(() =>
        {
            _hud.SendCommand(CommandType.CancleRemoveSlime);
        });
    }


    //
    private void HandleCancelSelectSlimePanel(CommandType type ,object _)
    {
        if(type != CommandType.CancleRemoveSlime) return;
        _selectionSequence?.Kill();
        _PanelSelectSlimes.SetActive(false);
        _btnTrigerRemove3SlimesSupport.gameObject.SetActive(true);
        _btnPause.interactable = true;
    }

    private Transform FindSelectionTitle()
    {
        TextMeshProUGUI[] labels =
            _PanelSelectSlimes.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.text.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
                return label.transform;
        }

        return null;
    }
    private void HandleUpdateCombo(CommandType cm , object data)
    {
        if(cm != CommandType.UpdateCombo) return;
        int combo = (int)data;
        _txtCombo.text = "X"+combo;
        _txtCombo.gameObject.SetActive(true);
        _transition.FxShowCombo(_txtCombo.transform);
        _timeDelay = 0;
    }
    private void WaitToHideCombo(float t)
    {
        _timeDelay += Time.deltaTime;
        if(_timeDelay < t) return;
        _timeDelay = 0;
        _txtCombo.gameObject.SetActive(false);
    }

    public void HandleFloatingScore(CommandType cm ,object  data)
    {
        if(cm != CommandType.FloatingScore) return;
        var (score, pos) = ((int score,Vector2 pos))data;
        FloatingScore floatingScore = ObjectPoolSystem.Instance.
        Order<FloatingScore>(_prefabFloatingScore,_prefabFloatingScore.PoolKey);
        floatingScore.transform.SetParent(transform,false);
        floatingScore.run(score,pos);
    }

    private void HandleUpdateScore(CommandType cm,object data)
    {
        if(cm != CommandType.AddScore) return;
        int targetScore = (int)data;

        _scoreTween?.Kill();

        if (_scoreCountDuration <= 0f)
        {
            SetDisplayedScore(targetScore);
            return;
        }

        _scoreTween = DOTween.To(
                () => _displayedScore,
                SetDisplayedScore,
                targetScore,
                _scoreCountDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .OnComplete(() => SetDisplayedScore(targetScore));
    }

    private void SetDisplayedScore(int score)
    {
        _displayedScore = score;
        _txtScore.text = HudManager.FormatScore(score);
    }
    
    private void HandleUpdatePreview(CommandType cm,object data)
    {
        if(cm != CommandType.UpdatePreview) return;
        Sprite img = (Sprite)data;
        _ImgPreview.sprite = img;
        _transition.FxShowNextSlime(_ImgPreview.transform);
    }

    private void HandleFlyPreviewToSpawn(CommandType cm, object data)
    {
        if (cm != CommandType.FlyPreviewToSpawn) return;

        var (spawnPosition, sprite, slimeScale, onComplete) =
            ((Vector3 spawnPosition, Sprite sprite, float slimeScale, Action onComplete))data;

        _transition.FxFlyNextSlimeToSpawn(
            _ImgPreview,
            spawnPosition,
            sprite,
            slimeScale,
            Camera.main,
            onComplete);
    }

    private void HandleUpdateHightScore(CommandType cm,object data)
    {
        if(cm != CommandType.UpdateHightScore) return;
        int hightscore = (int)data;
        _txtHightScore.text = HudManager.FormatScore(hightscore);
    }

    private void HandleUpdateRemove3SlimesSupport(CommandType cm,object data)
    {
        if(cm != CommandType.UpdateRemoveSlimesText) return;
        int count = (int)data;
        _txtRemove3Slimes.text = "Selected: "+count+"/3";
    }


}

[Serializable]
class PlayPanelTransition : UItransitionBase
{
    private const float MoveDuration = 0.55f;
    private const float HorizontalOffset = 220f;
    private const float ScoreVerticalOffset = 180f;
    private Vector3 _nextSlimeTargetScale;
    private bool _hasNextSlimeTargetScale;
    private Vector3 _comboTargetScale;
    private bool _hasComboTargetScale;

    public void PlayIntro(
        Transform score,
        Transform bestScore,
        Transform pause,
        Transform remove,
        Transform nextSlime,
        Transform mute)
    {
        FxMoveFrom(score, Vector3.up * ScoreVerticalOffset, MoveDuration, Ease.OutBack);
        DOTween.Sequence()
            .AppendInterval(0.1f)
            .Append(FxMoveFrom(
                bestScore,
                Vector3.up * ScoreVerticalOffset,
                MoveDuration,
                Ease.OutBack));
        FxMoveFrom(pause, Vector3.right * HorizontalOffset, MoveDuration);
        FxMoveFrom(remove, Vector3.left * HorizontalOffset, MoveDuration);
        FxShowNextSlime(nextSlime);
        if (mute != null)
            FxShowButtonPop(mute, 0.2f);
    }

    public Sequence FxShowNextSlime(Transform nextSlime)
    {
        DOTween.Kill(nextSlime);

        if (!_hasNextSlimeTargetScale)
        {
            _nextSlimeTargetScale = nextSlime.localScale;
            _hasNextSlimeTargetScale = true;
        }

        return FxPop(nextSlime, _nextSlimeTargetScale);
    }

    public Sequence FxShowCombo(Transform combo)
    {
        if (!_hasComboTargetScale)
        {
            _comboTargetScale = combo.localScale;
            _hasComboTargetScale = true;
        }

        return FxPop(combo, _comboTargetScale, 1.2f, 0.25f);
    }

    public Sequence FxFlyNextSlimeToSpawn(
        Image preview,
        Vector3 spawnWorldPosition,
        Sprite slimeSprite,
        float slimeScale,
        Camera worldCamera,
        Action onComplete)
    {
        const float duration = 0.32f;

        Image flyingPreview = CreateFlyingPreview(preview);
        RectTransform flyingRect = flyingPreview.rectTransform;

        CanvasGroup sourceGroup = GetOrAddCanvasGroup(preview.gameObject);
        sourceGroup.alpha = 0f;

        Vector3 targetPosition = WorldToCanvasPosition(
            preview.canvas,
            spawnWorldPosition,
            worldCamera);

        float targetScale = GetSpawnCanvasScale(
            flyingRect, preview.canvas, slimeSprite, slimeScale,
            spawnWorldPosition, worldCamera);

        return DOTween.Sequence()
            .Append(flyingRect.DOMove(targetPosition, duration).SetEase(Ease.InOutCubic))
            .Join(flyingRect.DOScale(Vector3.one * targetScale, duration)
                .SetEase(Ease.InOutQuad))
            .OnComplete(() =>
            {
                UnityEngine.Object.Destroy(flyingPreview.gameObject);
                sourceGroup.alpha = 1f;
                onComplete?.Invoke();
            })
            .SetUpdate(true);
    }

    private Image CreateFlyingPreview(Image preview)
    {
        Image flyingPreview = UnityEngine.Object.Instantiate(preview, preview.transform.parent);
        flyingPreview.rectTransform.position = preview.rectTransform.position;
        flyingPreview.rectTransform.localScale = _hasNextSlimeTargetScale
            ? _nextSlimeTargetScale
            : preview.rectTransform.localScale;
        flyingPreview.preserveAspect = true;
        flyingPreview.raycastTarget = false;
        return flyingPreview;
    }

    private static Vector3 WorldToCanvasPosition(
        Canvas canvas,
        Vector3 worldPosition,
        Camera worldCamera)
    {
        RectTransform canvasRect = (RectTransform)canvas.transform;
        Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
        Vector2 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect, screenPosition, canvasCamera, out Vector3 canvasPosition);
        return canvasPosition;
    }

    private static float GetSpawnCanvasScale(
        RectTransform preview,
        Canvas canvas,
        Sprite slimeSprite,
        float slimeScale,
        Vector3 spawnWorldPosition,
        Camera worldCamera)
    {
        float worldHeight = slimeSprite.bounds.size.y * slimeScale;
        Vector3 halfHeight = worldCamera.transform.up * worldHeight * 0.5f;
        float screenHeight = Mathf.Abs(
            worldCamera.WorldToScreenPoint(spawnWorldPosition + halfHeight).y -
            worldCamera.WorldToScreenPoint(spawnWorldPosition - halfHeight).y);
        return screenHeight / (preview.rect.height * canvas.scaleFactor);
    }

    public Sequence FxShowSelection(
        Transform title,
        Transform subtitle,
        Button cancel,
        Button remove)
    {
        const float titleDuration = 0.25f;

        cancel.interactable = false;
        remove.interactable = false;

        Sequence sequence = DOTween.Sequence()
            .Insert(0f, FxSlideFade(title, Vector3.up * 55f, titleDuration))
            .Insert(0.08f, FxSlideFade(
                subtitle, Vector3.up * 25f, titleDuration, Ease.OutQuad));

        float buttonsStart = 0.28f;
        sequence.Insert(buttonsStart, FxScaleFade(cancel.transform, 0.85f, 0.2f));
        sequence.Insert(buttonsStart + 0.1f,
            FxScaleFade(remove.transform, 0.85f, 0.2f));

        sequence.OnComplete(() =>
        {
            cancel.interactable = true;
            remove.interactable = true;
        });

        return sequence.SetUpdate(true);
    }

}
