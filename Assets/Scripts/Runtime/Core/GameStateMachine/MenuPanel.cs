using TMPro;
using UnityEngine;
using UnityEngine.UI;

class MenuPanel :  IState
{
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    [SerializeField]private Button _btnPlay;
    [SerializeField]private Sprite _SpriteBG;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _hud = GameManager.Instance.Hud;
        _hud.SetBackGround(_SpriteBG);
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _btnPlay.onClick.AddListener(BtnPlay); 
        _hud.OnCommand +=HandleUpdateHightScore;
    }

    public override void Exit()
    {
        _hud.OnCommand -=HandleUpdateHightScore;
        _btnPlay.onClick.RemoveListener(BtnPlay);
        this.gameObject.SetActive(false);
    }

    private void BtnPlay()
    {
        _hud.ChangeHud(StateType.Play);
        _hud.SendCommand(CommandType.Play);
    }
    private void HandleUpdateHightScore(CommandType cm,object data)
    {
        if(cm != CommandType.UpdateHightScore) return;
        int hightscore = (int)data;
        _txtHightScore.text = ""+hightscore;
    }

}