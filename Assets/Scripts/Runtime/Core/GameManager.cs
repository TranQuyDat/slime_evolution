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
        _hud.OnCommand += HandleBtnCommand;
    }

    void OnDestroy()
    {
        _hud.OnCommand -= HandleBtnCommand;
    }

    private void HandleBtnCommand(CommandType type, object _)
    {
        switch(type)
        {
            case  (CommandType.Play) :
            _gamePlay.StartPlay();
            break;
            case(CommandType.Pause):
            _gamePlay.PausePlay();
            break;
            case  (CommandType.Resume) :
            _gamePlay.ResumePlay();
            break;
            case  (CommandType.Home) :
            _gamePlay.StopAndClearPlay();
            break;
            case(CommandType.Reset):
            _gamePlay.ResetPlay();
            break;
        }
    }

    public void ShowGameOverHud() => _hud.ChangeHud(StateType.Over);

}
