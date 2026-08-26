using System;
using System.Xml.Serialization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class MenuPanel :  IState
{
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    [SerializeField]private Button _btnPlay;
    [SerializeField] private Button _btnMute;
    [SerializeField]private Sprite _SpriteBG;
    [SerializeField]private MenuPanelTransition _transition;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _hud = GameManager.Instance.Hud;
        _hud.SetBackGround(_SpriteBG);
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _btnPlay.onClick.AddListener(BtnPlay); 
        if (_btnMute != null)
        {
            _btnMute.onClick.AddListener(BtnMute);
            _hud.RefreshMuteButton(_btnMute);
        }
        _hud.OnCommand +=HandleUpdateHightScore;

        //fx
        _transition.OnStart();
        DOTween.Sequence()
        .Append(_transition.FxShowLogo(0.8f))
        .OnComplete(()=>_transition.FxLoopLogo(0.8f));

        DOTween.Sequence()
        .Append(_transition.FxShowBtn(0.8f))
        .Append(_transition.FxShowBestScore(0.8f))
        ;

        if (_btnMute != null)
            _transition.FxShowButtonPop(_btnMute.transform, 0.22f);

    }

    public override void Exit()
    {
        _hud.OnCommand -=HandleUpdateHightScore;
        _btnPlay.onClick.RemoveListener(BtnPlay);
        if (_btnMute != null)
            _btnMute.onClick.RemoveListener(BtnMute);
        this.gameObject.SetActive(false);
    }

    private void BtnMute()
    {
        _transition.FxClickBtn(_btnMute.transform, () =>
            _hud.ToggleMute(_btnMute));
    }

    private void BtnPlay()
    {
        DOTween.Sequence()
        .Append(_transition.FxClickBtn(_btnPlay.transform,0.15f))
        .OnComplete(() =>
        {
            _hud.ChangeHud(StateType.Play);
            _hud.SendCommand(CommandType.Play);
        });
    }
    private void HandleUpdateHightScore(CommandType cm,object data)
    {
        if(cm != CommandType.UpdateHightScore) return;
        int hightscore = (int)data;
        _txtHightScore.text = HudManager.FormatScore(hightscore);
    }

}

[Serializable]
class MenuPanelTransition : UItransitionBase
{
    [SerializeField]private Transform _logo;
    [SerializeField]private Image _imgBtnPlay;
    [SerializeField]private Transform _bestScoreTrns;
    private CanvasGroup _cgBestScore;

    public void OnStart()
    {
        if (_cgBestScore == null)
        {
            _cgBestScore = _bestScoreTrns.GetComponent<CanvasGroup>();

            if (_cgBestScore == null)
                _cgBestScore = _bestScoreTrns.gameObject.AddComponent<CanvasGroup>();
        }
        _logo.gameObject.SetActive(false);
        _imgBtnPlay.gameObject.SetActive(false);
        _bestScoreTrns.gameObject.SetActive(false);

    }

    public Sequence FxShowLogo(float duration)
    {
        Vector3 oriScale = _logo.localScale;
        float delta = 0.8f;
        _logo.localScale = oriScale*delta;
        _logo.gameObject.SetActive(true);

        DOTween.Kill(_logo);

        return DOTween.Sequence()
        .Append(_logo.DOScale(oriScale,duration)).SetEase(Ease.OutBack,1.3f)
        ;
    }

    public Sequence FxLoopLogo(float duration)
    {
        Vector3 oriPos  = _logo.position;
        Vector3 startPos = oriPos;
        startPos.y -= 0.1f;
        Vector3 endPos = oriPos;
        endPos.y += 0.1f;

        DOTween.Kill(_logo);

        return DOTween.Sequence()
        .Append(_logo.DOMove(startPos,duration*0.5f))
        .Append(_logo.DOMove(endPos,duration*0.5f))
        .SetLoops(-1,LoopType.Yoyo)
        ;
    }

    public Sequence FxShowBtn(float duration)
    {
        Transform btnTrns = _imgBtnPlay.transform;
        Vector3 oriPos = btnTrns.position;
        Vector3 startPos = oriPos;
        startPos.y -=0.8f;
        _imgBtnPlay.transform.position = startPos;

        Color cl = _imgBtnPlay.color;
        cl.a = 0;
        _imgBtnPlay.color = cl;

        btnTrns.gameObject.SetActive(true);

        DOTween.Kill(btnTrns);
        DOTween.Kill(_imgBtnPlay);

        return DOTween.Sequence()
        .Append(btnTrns.DOMove(oriPos,duration*1f))
        .Join(_imgBtnPlay.DOFade(1f,duration*1f))
        ;
    }

    public Sequence FxShowBestScore(float duration)
    {
        Vector3 oriPos = _bestScoreTrns.position;
        Vector3 startPos = oriPos;
        startPos.y -=0.8f;

        _cgBestScore.alpha = 0;
        _bestScoreTrns.position = startPos;
        _bestScoreTrns.gameObject.SetActive(true);

        DOTween.Kill(_cgBestScore);

        return DOTween.Sequence()
        .Append(_cgBestScore.DOFade(1f,duration))
        .Join(_bestScoreTrns.DOMove(oriPos,duration))
        ;
    }

}
