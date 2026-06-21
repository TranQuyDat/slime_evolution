using UnityEngine;
using UnityEngine.UI;

class PausePanel : IState
{
    [SerializeField]private Button _btnPlay;
    [SerializeField]private Button _btnReset;
    [SerializeField]private Button _btnHome;
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
    }

    public override void Exit()
    {     
        _btnPlay.onClick.RemoveListener(BtnPlay);
        _btnReset.onClick.RemoveListener(BtnReset);
        _btnHome.onClick.RemoveListener(BtnHome);
        this.gameObject.SetActive(false);
    }

    private void BtnPlay()
    {
        _hud.ChangeHud(StateType.Play);
        _hud.SendClickCommand(BtnCommand.Resume);
    } 
    private void BtnReset()
    {
        _hud.ChangeHud(StateType.Play);
        _hud.SendClickCommand(BtnCommand.Reset);
    } 
    private void BtnHome()
    { 
        _hud.ChangeHud(StateType.Menu);
        _hud.SendClickCommand(BtnCommand.Home);
    }

}