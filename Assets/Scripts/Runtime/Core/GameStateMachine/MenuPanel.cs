using TMPro;
using UnityEngine;
using UnityEngine.UI;

class MenuPanel :  IState
{
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    [SerializeField]private Button _btnPlay;
    private Sprite _SpriteBG;
    void Awake()
    {
        _SpriteBG = Resources.Load<Sprite>("SlimeSprites/bgMenu");
        _gameManager = GameManager.Instance;
        _hud = GameManager.Instance.Hud;
        _hud.SetBackGround(_SpriteBG);
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _btnPlay.onClick.AddListener(BtnPlay); 
    }

    public override void Exit()
    {
        _btnPlay.onClick.RemoveListener(BtnPlay);
        this.gameObject.SetActive(false);
    }

    private void BtnPlay()
    {
        _hud.ChangeHud(StateType.Play);
        _hud.SendClickCommand(BtnCommand.Play);
    }

}