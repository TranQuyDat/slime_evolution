using System;
using UnityEngine;

class GameMonetizeCallbackReceiver : MonoBehaviour
{
    private Action _onPause;
    private Action _onResume;

    public void Initialize(Action onPause, Action onResume)
    {
        _onPause = onPause;
        _onResume = onResume;
    }

    public void PauseGame() => _onPause?.Invoke();
    public void ResumeGame() => _onResume?.Invoke();
}
