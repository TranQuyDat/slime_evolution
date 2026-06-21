using UnityEngine;

[DefaultExecutionOrder(-1)]
class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputSystem _inputSystem;
    public GamePlay _gamePlay;
    [SerializeField] private HudManager _hud;

    public HudManager Hud => _hud;
    public GamePlay GamePlay => _gamePlay;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _gamePlay = GetComponentInChildren<GamePlay>();
    }
    void Start()
    {
        _hud.OnClickedBtn += HandleBtnCommand;
    }

    void OnDestroy()
    {
        _hud.OnClickedBtn -= HandleBtnCommand;
    }

    private void HandleBtnCommand(BtnCommand type)
    {
        switch(type)
        {
            case  (BtnCommand.Play) :
            _gamePlay.StartPlay();
            break;
            case(BtnCommand.Pause):
            _gamePlay.PausePlay();
            break;
            case  (BtnCommand.Resume) :
            _gamePlay.ResumePlay();
            break;
            case  (BtnCommand.Home) :
            _gamePlay.StopAndClearPlay();
            break;
            case(BtnCommand.Reset):
            _gamePlay.ResetPlay();
            break;
        }
    }

    public void ShowGameOverHud() => _hud.ChangeHud(StateType.Over);

}
