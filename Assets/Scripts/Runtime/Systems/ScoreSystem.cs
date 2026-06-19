using System;
using UnityEngine;

class ScoreSystem
{
    public event Action<int> OnChangeScore;
    private int _curScore;

    public int CurrentScore => _curScore;
    public ScoreSystem(int initScore)
    {
        _curScore = initScore;
    }
    public void AddScore(int i)
    {
        _curScore+=i;
        OnChangeScore?.Invoke(_curScore);
    }
}