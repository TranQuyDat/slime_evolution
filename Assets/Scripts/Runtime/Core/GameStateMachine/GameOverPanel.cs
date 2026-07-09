using UnityEngine;
using UnityEngine.UI;

class GameOverPanel:IState
{
    [SerializeField]private Button _btnReStart;
    [SerializeField]private Button _btnExit;
    [SerializeField]private Button _btnRevive;
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
    }

    public override void Exit()
    {
        _btnReStart.onClick.RemoveListener(BtnRestart);
        _btnExit.onClick.RemoveListener(BtnExit);
        _btnRevive.onClick.RemoveListener(BtnRevive);
        this.gameObject.SetActive(false);
    }

    private void BtnExit()
    {
        _hud.ChangeHud(StateType.Menu);
        _hud.SendCommand(CommandType.Home);
    }
    private void BtnRestart()
    {
        _hud.ChangeHud(StateType.Play);
        _hud.SendCommand(CommandType.Reset);
    }
    private void BtnRevive()
    {
        _hud.SendCommand(CommandType.Revive);
    }

}