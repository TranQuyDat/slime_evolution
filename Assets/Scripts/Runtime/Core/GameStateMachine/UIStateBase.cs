using UnityEngine;
using UnityEngine.UI;

abstract class UIStateBase : MonoBehaviour
{
    protected GameManager _gameManager;
    protected HudManager _hud;
    private BaseAudioEvent _BtnclickAudioEvent;
    private Button[] _buttons;

    void Start()
    {
        _buttons = GetComponentsInChildren<Button>(true);
        if (_buttons.Length == 0) return;

        _BtnclickAudioEvent = Resources.Load<BaseAudioEvent>("Events/BtnClick_Audio_Event");

        foreach (Button btn in _buttons)
        {
            btn.onClick.AddListener(soundClickBtn);
        }
    }

    void OnDestroy()
    {
        if (_buttons == null) return;

        foreach (Button btn in _buttons)
        {
            if (btn != null)
                btn.onClick.RemoveListener(soundClickBtn);
        }
    }

    private void soundClickBtn()
    {
        _BtnclickAudioEvent.Play();
    }

    public abstract void Enter();
    public abstract void Exit();
}
