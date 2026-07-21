using System.Collections;
using TMPro;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

class PlayPanel : IState
{
    [SerializeField]private Button _btnPause;
    [SerializeField]private Button _btnTrigerRemove3SlimesSupport;
    [SerializeField]private Button _btnRemove3SlimesSupport;
    [SerializeField]private Image _ImgPreview;
    [SerializeField]private TextMeshProUGUI _txtScore;
    [SerializeField]private TextMeshProUGUI _txtCombo;
    [SerializeField]private FloatingScore _prefabFloatingScore;
    [SerializeField]private TextMeshProUGUI _txtHightScore;
    [SerializeField]private TextMeshProUGUI _txtRemove3Slimes;

    [SerializeField]private GameObject _PanelSelectSlimes;

    private float _timeDelay;
    
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
    void Update()
    {
        if (_txtCombo.gameObject.activeSelf)
        {
            WaitToHideCombo(1.5f);
        }
    }
    public override void Enter()
    {
        this.gameObject.SetActive(true);
        _txtCombo.gameObject.SetActive(false);
        _btnTrigerRemove3SlimesSupport.gameObject.SetActive(true);
        _PanelSelectSlimes.SetActive(false);

        _btnPause.onClick.AddListener(BtnPause);
        _btnRemove3SlimesSupport.onClick.AddListener(BtnRemove3SlimesSupport);
        _btnTrigerRemove3SlimesSupport.onClick.AddListener(BtnTrigerRemove3SlimesSupport);

        _hud.OnCommand +=HandleUpdateHightScore;
        _hud.OnCommand +=HandleUpdatePreview;
        _hud.OnCommand +=HandleUpdateScore;
        _hud.OnCommand +=HandleUpdateRemove3SlimesSupport;
        _hud.OnCommand +=HandleUpdateCombo;
        _hud.OnCommand +=HandleFloatingScore;
    }

    public override void Exit()
    {
        _hud.OnCommand -=HandleUpdateHightScore;
        _hud.OnCommand -=HandleUpdatePreview;
        _hud.OnCommand -=HandleUpdateScore;
        _hud.OnCommand -=HandleUpdateCombo;

        _btnPause.onClick.RemoveListener(BtnPause);
        _btnRemove3SlimesSupport.onClick.RemoveListener(BtnRemove3SlimesSupport);
        _btnTrigerRemove3SlimesSupport.onClick.RemoveListener(BtnTrigerRemove3SlimesSupport);
    }
    private void HandleChangeHud(StateType type)
    {
        if(type != StateType.Menu) return;
        this.gameObject.SetActive(false);
        _hud.OnChangeHud -=HandleChangeHud;
        _txtCombo.gameObject.SetActive(false);
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
    private void HandleUpdateCombo(CommandType cm , object data)
    {
        if(cm != CommandType.UpdateCombo) return;
        int combo = (int)data;
        _txtCombo.text = "X"+combo;
        _txtCombo.gameObject.SetActive(true);
        _timeDelay = 0;
    }
    private void WaitToHideCombo(float t)
    {
        _timeDelay += Time.deltaTime;
        if(_timeDelay < t) return;
        _timeDelay = 0;
        _txtCombo.gameObject.SetActive(false);
    }

    public void HandleFloatingScore(CommandType cm ,object  data)
    {
        if(cm != CommandType.FloatingScore) return;
        var (score, pos) = ((int score,Vector2 pos))data;
        FloatingScore floatingScore = ObjectPoolSystem.Instance.
        Order<FloatingScore>(_prefabFloatingScore,_prefabFloatingScore.PoolKey);
        floatingScore.transform.SetParent(transform,false);
        floatingScore.run(score,pos);
    }

    private void HandleUpdateScore(CommandType cm,object data)
    {
        if(cm != CommandType.AddScore) return;
        int score  = (int)data;
        _txtScore.text = ""+score;
    }
    
    private void HandleUpdatePreview(CommandType cm,object data)
    {
        if(cm != CommandType.UpdatePreview) return;
        Sprite img = (Sprite)data;
        _ImgPreview.sprite = img;
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