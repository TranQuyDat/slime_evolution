using System;
using UnityEngine;

class StateMachine
{
    private UIStateBase _curState;

    public void ChangeState(UIStateBase newState)
    {
        _curState?.Exit();
        _curState = newState;
        _curState.Enter();
    }
}
