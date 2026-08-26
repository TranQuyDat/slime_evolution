using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;
public enum StateType
{
    Menu = 0,
    Play = 1,
    Pause = 2,
    Over = 3,
}
public enum CommandType
{
    //game flow command
    Play,  Pause,  Resume, Reset, Home,
    //game event command
    AddScore,UpdateHightScore,Revive,Remove3Slimes,TrigerRemove3Slimes,CancleRemoveSlime,
    UpdateRemoveSlimesText,UpdateCombo,UpdatePreview,FloatingScore,FlyPreviewToSpawn
}
class HudManager : MonoBehaviour
{
    public static HudManager Instance { get; private set; }
    [SerializeField]private GameStateDatabase _gameStatedatabase;
    [SerializeField]private Image _bg;
    [Header("Audio")]
    [Range(0f, 1f)]
    [SerializeField] private float _mutedButtonAlpha = 0.45f;
    private StateMachine _stateMachine;
    private UIElement[] _uiElems;
    private SortedList<StateType,IState> _uiStates;
    public event Action<StateType> OnChangeHud ;
    public event Action<CommandType,object> OnCommand ;

    void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        FetchAndSortUIElements();
        _uiStates = new SortedList<StateType, IState>();
        _stateMachine = new StateMachine();
        Init();
        ChangeHud(StateType.Menu);
    }


    public void ChangeHud(StateType type)
    { 
        if(!_uiStates.TryGetValue(type,out IState s) || s==null)
        {
            GameObject obj = Instantiate(_gameStatedatabase.Uis[(int)type],transform);
            s = obj.GetComponent<IState>();
            _uiStates[type] = s;
            FetchAndSortUIElements();
        }
        _stateMachine.ChangeState(s);
        OnChangeHud?.Invoke(type);
        
    }

    private void Init()
    {
        foreach(Transform t in transform)
        {
            if(Enum.TryParse(t.name,true,out StateType type))
            {
                IState s = t.GetComponent<IState>();
                _uiStates[type] = s;
            }
        }
    }
    public void SendCommand(CommandType cm,object data = null)
    {
        OnCommand?.Invoke(cm,data);
    }

    public GameObject GetUiByNameElement(string nameElement)
    {
        
        int left = 0;
        int right = _uiElems.Length - 1;
        while (left <= right)
        {
            int mid = (right + left) / 2;
            int cmp = string.Compare(_uiElems[mid].CustomName, nameElement, StringComparison.OrdinalIgnoreCase);
            if(cmp == 0) return _uiElems[mid].gameObject;
            if(cmp < 0) left = mid+1;
            else right = mid-1;
        }
        return null;
    }

    public void FetchAndSortUIElements()
    {
        _uiElems = transform.GetComponentsInChildren<UIElement>(true)
            .OrderBy(t => t.CustomName)
            .ToArray();
    }

    public void SetBackGround(Sprite sprite) => _bg.sprite = sprite;

    public void ToggleMute(Button button)
    {
        AudioManager.Instance.ToggleMute();
        RefreshMuteButton(button);
    }

    public void RefreshMuteButton(Button button)
    {
        if (button == null || AudioManager.Instance == null) return;

        Image image = button.image != null
            ? button.image
            : button.GetComponentInChildren<Image>(true);
        if (image == null) return;

        Color color = image.color;
        color.a = AudioManager.Instance.IsMuted ? _mutedButtonAlpha : 1f;
        image.color = color;
    }

    public static string FormatScore(int score)
    {
        string[] suffixes = { "", "K", "M", "B" };
        double value = score;
        int suffixIndex = 0;

        while (System.Math.Abs(value) >= 1000d && suffixIndex < suffixes.Length - 1)
        {
            value /= 1000d;
            suffixIndex++;
        }

        string format = System.Math.Abs(value) >= 100d ? "0" : "0.##";
        return value.ToString(format, CultureInfo.InvariantCulture) + suffixes[suffixIndex];
    }
}
