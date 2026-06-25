using UnityEngine;
using UnityEngine.UI;
abstract class IState : MonoBehaviour
{
    protected GameManager _gameManager;
    protected HudManager _hud;
    private BaseAudioEvent _BtnclickAudioEvent;
    void Start()
    {
        Button[] btns = GetComponentsInChildren<Button>();
        if(btns == null || btns.Length <= 0) return;
        _BtnclickAudioEvent = Resources.Load<BaseAudioEvent>("Events/BtnClick_Audio_Event");
        

        foreach(Button btn in btns)
        {
            btn.onClick.AddListener(soundClickBtn);
        }
    }

    private void soundClickBtn()
    {
        AudioSource source = AudioManager.Instance.getSource();
        _BtnclickAudioEvent.play(source);
        AudioManager.Instance.StopSfxUntillFinish(source);
    }

    public abstract void Enter();
    public abstract void Exit();
}
