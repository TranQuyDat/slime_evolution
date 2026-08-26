using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

class GameOverPanel:IState
{
    [SerializeField]private Button _btnReStart;
    [SerializeField]private Button _btnExit;
    [SerializeField]private Button _btnRevive;
    [SerializeField] private Button _btnMute;
    private readonly GameOverPanelTransition _transition = new GameOverPanelTransition();
    private Sequence _introSequence;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _hud = _gameManager.Hud;
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _btnReStart.onClick.AddListener(BtnRestart);
        _btnExit.onClick.AddListener(BtnExit);
        _btnRevive.onClick.AddListener(BtnRevive);
        if (_btnMute != null)
        {
            _btnMute.onClick.AddListener(BtnMute);
            _hud.RefreshMuteButton(_btnMute);
        }

        Transform title = transform.Find("txt_title");
        _introSequence = _transition.Play(
            gameObject,
            title,
            _btnRevive,
            _btnReStart,
            _btnExit,
            _btnMute);
    }

    public override void Exit()
    {
        _introSequence?.Kill();
        _btnReStart.onClick.RemoveListener(BtnRestart);
        _btnExit.onClick.RemoveListener(BtnExit);
        _btnRevive.onClick.RemoveListener(BtnRevive);
        if (_btnMute != null)
            _btnMute.onClick.RemoveListener(BtnMute);
        this.gameObject.SetActive(false);
    }
    private void BtnMute()
    {
        _transition.FxClickBtn(_btnMute.transform, () =>
            _hud.ToggleMute(_btnMute));
    }

    private void BtnExit()
    {
        _transition.FxClickBtn(_btnExit.transform, () =>
        {
            _hud.ChangeHud(StateType.Menu);
            _hud.SendCommand(CommandType.Home);
        });
    }
    private void BtnRestart()
    {
        _transition.FxClickBtn(_btnReStart.transform, () =>
        {
            _hud.ChangeHud(StateType.Play);
            _hud.SendCommand(CommandType.Reset);
        });
    }
    private void BtnRevive()
    {
        _transition.FxClickBtn(
            _btnRevive.transform,
            () => _hud.SendCommand(CommandType.Revive));
    }

}

[Serializable]
class GameOverPanelTransition : UItransitionBase
{
    private const float Stagger = 0.1f;

    public Sequence Play(
        GameObject panel,
        Transform title,
        Button revive,
        Button reset,
        Button home,
        Button mute)
    {
        DOTween.Kill(panel.transform);

        Sequence sequence = DOTween.Sequence()
            .Insert(0f, FxFadeOverlay(panel.transform, 0.68f, 0.22f))
            .Insert(0f, FxScale(title, 1.4f, 0.3f))
            .Insert(Stagger, FxScale(revive.transform, 0.7f, 0.25f))
            .Insert(Stagger * 2f,
                FxSlideFade(reset.transform, Vector3.down * 70f, 0.25f))
            .Insert(Stagger * 3f,
                FxSlideFade(home.transform, Vector3.down * 70f, 0.25f));

        if (mute != null)
            sequence.Insert(Stagger * 4f,
                FxShowButtonPop(mute.transform, 0.2f));

        return sequence;
    }

}
