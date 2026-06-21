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
    }

    public override void Exit()
    {
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
        _hud.SendClickCommand(BtnCommand.Pause);
    }

}