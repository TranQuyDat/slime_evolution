using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ReviveAction : SupportAction
{
    private PitController _pitCtrl;
    private GamePlay _gamePlay;
    private GameManager _gameManager;
    public ReviveAction (PitController pitCtrl)
    {
        _gameManager = GameManager.Instance;
        _gamePlay = _gameManager.GamePlay;
        _pitCtrl = pitCtrl;
    }

    public override void OnEnter(){}

    public override void OnAction(Action Oncomplete)
    {
        List<Slime> slimes =  _pitCtrl.GetAllContents<Slime>().ToList();
        slimes.Sort((a, b) => a.transform.position.y.
        CompareTo(b.transform.position.y));
        int count = slimes.Count/2;
        //remove 1/2 slimes
        for(int i = 0 ;i< count; i++)
        {
            slimes[i].Destroy();  
        }
        _gameManager.Hud.ChangeHud(StateType.Play);
        Oncomplete?.Invoke();

    }

    public override void OnFinish(){}
    
}