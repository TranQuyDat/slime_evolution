using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-1)]
 class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]private InputSystem _inputSystem;
    [SerializeField]private GamePlay _gamePlay;
    [SerializeField] private HudManager _hud;
    [SerializeField]private SimpleAudioEvent _bgmAudioEvent;

    private MonetizationManager _monetizationMngr;
    private SaveSystem _saveSystem;
    private int _hightScore;
    private Dictionary<CommandType, Action> _commandMap;
    private GoalSystem _goalSystem;

    public HudManager Hud => _hud;
    public GamePlay GamePlay => _gamePlay;
    public InputSystem InputSystem => _inputSystem;
    public GoalSystem GoalSystem => _goalSystem;
    public SaveSystem SaveSystem => _saveSystem;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _saveSystem = new SaveSystem();
        _saveSystem.Provider = new PlayerPrefsProvider();

        _gamePlay = GetComponentInChildren<GamePlay>();
        _monetizationMngr = FindFirstObjectByType<MonetizationManager>();

        _goalSystem = new GoalSystem();
        _goalSystem.Initialize();
        
    }

    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        _monetizationMngr = MonetizationManager.Instance;

        SetupCommandMap();
        
        _hightScore = _saveSystem.Load<int>("hightscore");

        _hud.OnCommand += HandleBtnCommand;
        _hud.OnChangeHud += HandleLoadDataHud;
        
        _gamePlay.ScoreSystem.OnChangeScore += HandleHightScoreChange;
        _gamePlay.ScoreSystem.OnChangeScore += updateCurScoreinHud;

        _bgmAudioEvent.Play();
        _hud.SendCommand(CommandType.UpdateHightScore, _hightScore);
    }

    void OnDestroy()
    {
        _hud.OnCommand -= HandleBtnCommand;
        _hud.OnChangeHud -= HandleLoadDataHud;
       
        _gamePlay.ScoreSystem.OnChangeScore -= HandleHightScoreChange;
        _gamePlay.ScoreSystem.OnChangeScore -= updateCurScoreinHud;
        
    }

    private void SetupCommandMap()
    {
        _commandMap = new Dictionary<CommandType, Action>
        {
            { CommandType.Play, 
                () => { _gamePlay.StartPlay();
                        _hud.SendCommand(CommandType.AddScore,0); } },

            { CommandType.Pause, _gamePlay.PausePlay },

            { CommandType.Resume, _gamePlay.ResumePlay },

            { CommandType.Home, _gamePlay.StopAndClearPlay },

            { CommandType.Reset, _gamePlay.ResetPlay },

            { CommandType.Revive, 
                ()=>{ RequestSupportAd(_gamePlay.ReviveSupport); } },

            { CommandType.TrigerRemove3Slimes, 
                ()=>{ RequestSupportAd(_gamePlay.TrigerRemoveSlimesSupport); } },

            { CommandType.Remove3Slimes,()=> _gamePlay.RemoveSlimesSupport(
                ()=>_hud.SendCommand(CommandType.CancleRemoveSlime)) },

            { CommandType.CancleRemoveSlime, _gamePlay.CancleSlimeSupport },

            
        };
    }

    public void RunFloatingScore( (int,Vector2) data )
    {
        _hud.SendCommand(CommandType.FloatingScore,data);
    }

    // Handle button Command
    private void HandleBtnCommand(CommandType type, object _)
    {
        if(_commandMap == null) return;
        _commandMap.TryGetValue(type, out Action action);
        action?.Invoke();
    }

    private void HandleLoadDataHud(StateType type)
    {
        if(type != StateType.Menu && type != StateType.Play)
            return;
        int hightScore = _saveSystem.Load<int>("hightscore");
        _hud.SendCommand(CommandType.UpdateHightScore,hightScore);

        if(type != StateType.Menu) return;

        var achivements =  _goalSystem.GetQuests(GoalData.Type.Achievement);
        foreach(var item in achivements)
        {
            float progress = _saveSystem.Load<float>($"{item.goalId}_{item.quest.Id}");
            if(item.quest.Type == QuestionType.Best_Score)
            {
                progress = hightScore;
            }
            _hud.SendCommand(CommandType.InitGoal,(item.quest.Type, item.quest.QuestionText, progress, item.quest.Target));    
        }
         
    }

    public void ShowGameOverHud() => _hud.ChangeHud(StateType.Over);

    public void UpdatePreviewHud(Sprite sprite)
    {
        _hud.SendCommand(CommandType.UpdatePreview,sprite);
    }
    
    public void UpdateProgressHud(int value)
    {
        _hud.SendCommand(CommandType.UpdateProgress, value);
    }
    
    public void FlyPreviewToSpawn(
        Vector3 spawnPosition,
        Sprite sprite,
        float slimeScale,
        Action onComplete)
    {
        _hud.SendCommand(
            CommandType.FlyPreviewToSpawn,
            (spawnPosition, sprite, slimeScale, onComplete));
    }
    
    public void updateComboHud(int combo)
    {
        _hud.SendCommand(CommandType.UpdateCombo,combo);
    }
    
    private void updateCurScoreinHud(int score)
    {
        _hud.SendCommand(CommandType.AddScore,score);
        UpdateGoalHud(QuestionType.Reach_Score,score);
        Debug.Log($"Event Update Score{score}");
    }

    public void InitGoalHud(QuestionType type, string goalText, float targetValue)
    {
        _hud.SendCommand(CommandType.InitGoal,( type, goalText, targetValue));
    }

    public void UpdateGoalHud(QuestionType type ,float progressValue)
    {
        _hud.SendCommand(CommandType.UpdateGoal,(type ,progressValue));
        _goalSystem.UpdateProgress(type,progressValue);
        Debug.Log($"Event Update Goal{type}: {progressValue}");
    }

    private void RequestSupportAd(Action onComplete)
    {
        _gamePlay.PausePlay();
        _monetizationMngr.ShowAd(() =>
        {
            onComplete?.Invoke();
            _gamePlay.ResumePlay();
        });
    }

    private void HandleHightScoreChange(int score)
    {
        if(score <= _hightScore) return;
        _saveSystem.Save<int>(score,"hightscore");
        _hud.SendCommand(CommandType.UpdateHightScore,score);
        UpdateGoalHud(QuestionType.Best_Score,score);

        Debug.Log($"Event Update Hight Score{score}");
    }

    [ContextMenu("Delete All Data Saved")]
    private void ResetDataSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
