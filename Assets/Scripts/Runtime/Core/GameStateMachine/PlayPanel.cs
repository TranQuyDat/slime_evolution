using TMPro;
using UnityEngine;
using UnityEngine.UI;

class PlayPanel : IState
{
    [SerializeField]private Button _btnPause;
    [SerializeField]private TextMeshProUGUI _txtScore;
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    private Sprite _spriteBG;
    void Awake()
    {
        _spriteBG = Resources.Load<Sprite>("SlimeSprites/bgPlay");
        _gameManager = GameManager.Instance;
        _hud = GameManager.Instance.Hud;
        _hud.SetBackGround(_spriteBG);
    }
    void OnEnable()
    {
        _hud.OnChangeHud +=HandleChangeHud;
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _btnPause.onClick.AddListener(BtnPause);
        _hud.OnCommand +=HandleUpdateHightScore;
        _hud.OnCommand +=HandleUpdateScore;
    }

    public override void Exit()
    {
        _hud.OnCommand -=HandleUpdateHightScore;
        _hud.OnCommand -=HandleUpdateScore;
        _btnPause.onClick.RemoveListener(BtnPause);
    }
    private void HandleChangeHud(StateType type)
    {
        if(type != StateType.Menu) return;
        this.gameObject.SetActive(false);
        _hud.OnChangeHud -=HandleChangeHud;
    }
    private void BtnPause() 
    { 
        _hud.ChangeHud(StateType.Pause);
        _hud.SendCommand(CommandType.Pause);
    }

    private void HandleUpdateScore(CommandType cm,object data)
    {
        if(cm != CommandType.AddScore) return;
        int score  = (int)data;
        _txtScore.text = ""+score;
    }
    
    private void HandleUpdateHightScore(CommandType cm,object data)
    {
        if(cm != CommandType.UpdateHightScore) return;
        int hightscore = (int)data;
        _txtHightScore.text = ""+hightscore;
    }

}