using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

class PausePanel : IState
{
    [SerializeField]private Button _btnPlay;
    [SerializeField]private Button _btnReset;
    [SerializeField]private Button _btnHome;
    private readonly PausePanelTransition _transition = new PausePanelTransition();
    private Sequence _introSequence;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _hud = _gameManager.Hud;
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _btnPlay.onClick.AddListener(BtnPlay);
        _btnReset.onClick.AddListener(BtnReset);
        _btnHome.onClick.AddListener(BtnHome);

        _introSequence = _transition.Play(
            gameObject, _btnHome, _btnPlay, _btnReset);
    }

    public override void Exit()
    {
        _introSequence?.Kill();
        _btnPlay.onClick.RemoveListener(BtnPlay);
        _btnReset.onClick.RemoveListener(BtnReset);
        _btnHome.onClick.RemoveListener(BtnHome);
        this.gameObject.SetActive(false);
    }

    private void BtnPlay()
    {
        _transition.FxClickBtn(_btnPlay.transform, () =>
        {
            _hud.ChangeHud(StateType.Play);
            _hud.SendCommand(CommandType.Resume);
        });
    } 
    private void BtnReset()
    {
        _transition.FxClickBtn(_btnReset.transform, () =>
        {
            _hud.ChangeHud(StateType.Play);
            _hud.SendCommand(CommandType.Reset);
        });
    } 
    private void BtnHome()
    {
        _transition.FxClickBtn(_btnHome.transform, () =>
        {
            _hud.ChangeHud(StateType.Menu);
            _hud.SendCommand(CommandType.Home);
        });
    }

}

[Serializable]
class PausePanelTransition : UItransitionBase
{
    public Sequence Play(
        GameObject panel,
        Button home,
        Button continueButton,
        Button reset)
    {
        Transform[] buttons =
        {
            home.transform,
            continueButton.transform,
            reset.transform
        };
        Sequence sequence = DOTween.Sequence()
            .Insert(0f, FxFadeOverlay(panel.transform, 0.55f, 0.15f));

        for (int i = 0; i < buttons.Length; i++)
        {
            float delay = i * 0.04f;
            Ease scaleEase = buttons[i] == continueButton.transform
                ? Ease.OutBack
                : Ease.OutQuad;

            sequence.Insert(delay, FxSlideScale(
                buttons[i],
                Vector3.down * 40f,
                0.9f,
                0.2f,
                Ease.OutCubic,
                scaleEase));
        }

        return sequence;
    }
}
