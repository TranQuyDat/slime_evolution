using System;
using System.Collections.Generic;
using CrazyGames;
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

    public HudManager Hud => _hud;
    public GamePlay GamePlay => _gamePlay;
    public InputSystem InputSystem => _inputSystem;
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

    async void Start()
    {
        await InitializeSaveProvider();
        InitializeGame();
    }

    private async System.Threading.Tasks.Task InitializeSaveProvider()
    {
#if UNITY_EDITOR || UNITY_WEBGL
        if (!CrazySDK.IsAvailable) return;

        try
        {
            await CrazySDK.InitAsync();
            _saveSystem.Provider = new CrazyGamesSaveProvider();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"CrazyGames save is unavailable. Using PlayerPrefs instead. {exception.Message}");
        }
#endif
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
                ()=>{ RequestSupportRewardedAd(_gamePlay.ReviveSupport); } },
            { CommandType.TrigerRemove3Slimes, 
                ()=>{ RequestSupportRewardedAd(_gamePlay.TrigerRemoveSlimesSupport); } },
            { CommandType.Remove3Slimes,()=> _gamePlay.RemoveSlimesSupport(
                ()=>_hud.SendCommand(CommandType.CancleRemoveSlime)) },
            { CommandType.CancleRemoveSlime, _gamePlay.CancleSlimeSupport },
        };
    }

    public void RunFloatingScore( (int,Vector2) data )
    {
        _hud.SendCommand(CommandType.FloatingScore,data);
    }
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
    }

    public void ShowGameOverHud() => _hud.ChangeHud(StateType.Over);
    public void UpdatePreviewHud(Sprite sprite)
    {
        _hud.SendCommand(CommandType.UpdatePreview,sprite);
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
    }

    private void RequestSupportRewardedAd(Action sp)
    {
        _gamePlay.PausePlay();
        _monetizationMngr.ShowAdReward(()=>
        {
            sp?.Invoke();
            _gamePlay.ResumePlay();
        });
    }

    private void HandleHightScoreChange(int score)
    {
        if(score <= _hightScore) return;
        _saveSystem.Save<int>(score,"hightscore");
        _hud.SendCommand(CommandType.UpdateHightScore,score);
    }
}
