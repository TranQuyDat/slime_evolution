using TMPro;
using UnityEngine;
using UnityEngine.UI;

class PlayPanel : IState
{
    [SerializeField]private Button _btnPause;
    [SerializeField]private Button _btnTrigerRemove3SlimesSupport;
    [SerializeField]private Button _btnRemove3SlimesSupport;
    [SerializeField]private TextMeshProUGUI _txtScore;
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    [SerializeField]private TextMeshProUGUI _txtRemove3Slimes;

    [SerializeField]private GameObject _PanelSelectSlimes;
    
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
        _btnTrigerRemove3SlimesSupport.gameObject.SetActive(true);
        _PanelSelectSlimes.SetActive(false);
        _btnPause.onClick.AddListener(BtnPause);
        _btnRemove3SlimesSupport.onClick.AddListener(BtnRemove3SlimesSupport);
        _btnTrigerRemove3SlimesSupport.onClick.AddListener(BtnTrigerRemove3SlimesSupport);

        _hud.OnCommand +=HandleUpdateHightScore;
        _hud.OnCommand +=HandleUpdateScore;
        _hud.OnCommand +=HandleUpdateRemove3SlimesSupport;
    }

    public override void Exit()
    {
        _hud.OnCommand -=HandleUpdateHightScore;
        _hud.OnCommand -=HandleUpdateScore;

        _btnPause.onClick.RemoveListener(BtnPause);
        _btnRemove3SlimesSupport.onClick.RemoveListener(BtnRemove3SlimesSupport);
        _btnTrigerRemove3SlimesSupport.onClick.RemoveListener(BtnTrigerRemove3SlimesSupport);
    }
    private void HandleChangeHud(StateType type)
    {
        if(type != StateType.Menu) return;
        this.gameObject.SetActive(false);
        _hud.OnChangeHud -=HandleChangeHud;
    }

    // buttons
    private void BtnPause() 
    { 
        _hud.ChangeHud(StateType.Pause);
        _hud.SendCommand(CommandType.Pause);
    }
    private void BtnTrigerRemove3SlimesSupport()
    {
        _PanelSelectSlimes.SetActive(true);
        _btnTrigerRemove3SlimesSupport.gameObject.SetActive(false);
        _btnPause.interactable = false;
        _hud.SendCommand(CommandType.TrigerRemove3Slimes);
    }
    private void BtnRemove3SlimesSupport()
    {
        _PanelSelectSlimes.SetActive(false);
        _btnTrigerRemove3SlimesSupport.gameObject.SetActive(true);
        _hud.SendCommand(CommandType.Remove3Slimes);
        _btnPause.interactable = true;
    }


    //
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

    private void HandleUpdateRemove3SlimesSupport(CommandType cm,object data)
    {
        if(cm != CommandType.UpdateRemoveSlimesText) return;
        int count = (int)data;
        _txtRemove3Slimes.text = ""+count+"/3";
    }


}