using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
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
    AddScore,UpdateHightScore,Revive,Remove3Slimes,TrigerRemove3Slimes,
    UpdateRemoveSlimesText,
}
class HudManager : MonoBehaviour
{
    [SerializeField]private GameStateDatabase _gameStatedatabase;
    [SerializeField]private Image _bg;
    private StateMachine _stateMachine;
    private SortedList<StateType,IState> _uiStates;
    public event Action<StateType> OnChangeHud ;
    public event Action<CommandType,object> OnCommand ;  

    void Start()
    {
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

    public void SetBackGround(Sprite sprite) => _bg.sprite = sprite;
}