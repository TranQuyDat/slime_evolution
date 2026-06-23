using UnityEngine;

[DefaultExecutionOrder(-1)]
class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]private InputSystem _inputSystem;
    [SerializeField]private GamePlay _gamePlay;

    public InputSystem InputSystem => _inputSystem;
    [SerializeField] private HudManager _hud;

    public HudManager Hud => _hud;
    public GamePlay GamePlay => _gamePlay;

    private SaveSystem _saveSystem;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _gamePlay = GetComponentInChildren<GamePlay>();
        _saveSystem = new SaveSystem();
        _saveSystem.Provider = new PlayerPrefsProvider();
    }
    void Start()
    {
        _hud.OnCommand += HandleBtnCommand;
        _hud.OnChangeHud += HandleLoadDataHud;
        _gamePlay.ScoreSystem.OnChangeScore += HandleHightScoreChange;
        _gamePlay.ScoreSystem.OnChangeScore += updateCurScoreinHud;
    }

    void OnDestroy()
    {
        _hud.OnCommand -= HandleBtnCommand;
        _hud.OnChangeHud -= HandleLoadDataHud;
        _gamePlay.ScoreSystem.OnChangeScore -= HandleHightScoreChange;
        _gamePlay.ScoreSystem.OnChangeScore -= updateCurScoreinHud;
        
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
    private void HandleLoadDataHud(StateType type)
    {
        if(type != StateType.Menu && type != StateType.Play)
            return;
        int hightScore = _saveSystem.Load<int>("hightscore");
        _hud.SendCommand(CommandType.UpdateHightScore,hightScore);
    }

    public void ShowGameOverHud() => _hud.ChangeHud(StateType.Over);
    private void updateCurScoreinHud(int score)
    {
        _hud.SendCommand(CommandType.AddScore,score);
    }
    private void HandleHightScoreChange(int score)
    {
        int hightScore = _saveSystem.Load<int>("hightscore");
        if(score <= hightScore) return;
        _saveSystem.Save<int>(score,"hightscore");
        _hud.SendCommand(CommandType.UpdateHightScore,score);
    }
}
