using System;
using UnityEngine;

class ComboSystem
{
    public event Action<int> OnComboChanged;
    public event Action OnComboReset;
    private int _comboCount = 0;
    private float _comboTimer = 0;

    public int ComBoCount => _comboCount;
    public void AddComboCount()
    {
        _comboCount ++;
        _comboTimer = 0;
        OnComboChanged?.Invoke(_comboCount);
    }

    public void ResetComboByTime(float time)
    {
        if(_comboCount <= 0) return;
        _comboTimer +=Time.deltaTime;
        if(_comboTimer > time)
        {
            _comboCount = 0;
            _comboTimer = 0;
            OnComboReset?.Invoke();
        } 
        
    }
}