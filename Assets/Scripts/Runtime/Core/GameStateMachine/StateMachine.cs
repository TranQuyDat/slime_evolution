using System;
using UnityEngine;

class StateMachine
{
    private IState _curState;

    public void ChangeState(IState newState)
    {
        _curState?.Exit();
        _curState = newState;
        _curState.Enter();
    }
}