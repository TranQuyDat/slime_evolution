using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

class GamePlay : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem ;
    [SerializeField] private SpawnSystem _spawnSystem;
    [SerializeField] private GameObject _pitPrefab;
    [SerializeField] private ComboWindow _comboWindowPrefab;
    [SerializeField] private float _dragThreshold = 0.5f;

    private GameManager _gameManager;
    private ScoreSystem _scoreSystem;
    private ComboSystem _comboSystem;
    private PitController _pitCtrl;
    private ComboWindow _CombowindowCtrl;
    private float _timeDelay = 0f;
    public bool IsGameOver {get;private set;}
    private bool _isDropSlime;
    public ScoreSystem ScoreSystem => _scoreSystem;
    private bool _canPlay;
    private bool _trigerRemoveSlime;
    private SupportAction _reviveAction;
    private SupportAction _removeSlimeAction;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _scoreSystem = new ScoreSystem(); 
        _comboSystem = new ComboSystem();
       
    }
    void Start()
    {
        _inputSystem.BindAction(KeyCode.Mouse0,DropSlime);
        _isDropSlime = false;
        _canPlay = false;
    }

    void Update()
    {
        if(!_canPlay) return;
        DragSlime_X();

        if(_trigerRemoveSlime)
        {
            _removeSlimeAction.OnUpdate();
            return;
        }

        if(_pitCtrl.HadOverflowed)
        {
            CheckGameOverByTimeout(3f);
            return;
        }
        
        
        if(!_spawnSystem._canSpawn && _isDropSlime)
            waitToSpawn(3f);

        _comboSystem.ResetComboByTime(1.5f);

    }

    public void StartPlay()
    {
        if(_pitCtrl == null)
        {
            //create pit and set pos pit
            GameObject pitObj = Instantiate(_pitPrefab,transform); 
            _pitCtrl = pitObj.GetComponent<PitController>();

            CompositeCollider2D compositeCol = pitObj.GetComponent<CompositeCollider2D>();
            float pitSizeY = compositeCol.bounds.size.y/2f;
            Vector2 pos =  Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0f,10f));
            pos.y += pitSizeY;
            pitObj.transform.position = pos; // set pos for pit
        }
        if(_CombowindowCtrl == null) 
            _CombowindowCtrl = Instantiate(_comboWindowPrefab,_gameManager.Hud.transform);
        _pitCtrl.gameObject.SetActive(true);
        _CombowindowCtrl.gameObject.SetActive(false);

        if(_reviveAction == null)
            _reviveAction = new ReviveAction(_pitCtrl);
        
        if(_removeSlimeAction == null)
            _removeSlimeAction = new RemoveSlimeAction(_pitCtrl,_inputSystem);

        _comboSystem.OnComboChanged += HandleComboChange;
        _comboSystem.OnComboReset += HandleComboReset;
        _spawnSystem._canSpawn = true;
        ResetVariables();
        _scoreSystem.SetScore(0);
        waitToSpawn(0f);

    }
    public void PausePlay() => _canPlay = false;
    public void ResumePlay() => _canPlay = true;
    
    public void ResetPlay()
    {
        IsGameOver = false;
        _scoreSystem.SetScore(0);
        _spawnSystem.Reset();
        _pitCtrl.ClearAllContent();
        ResetVariables();
        waitToSpawn(0f);
        _removeSlimeAction.OnFinish();
    }

    public void StopAndClearPlay()
    {
        _canPlay = false;
        _spawnSystem.Reset();
        _pitCtrl.ClearAllContent();
        _timeDelay = 0f;
        _pitCtrl.gameObject.SetActive(false);
        
        _comboSystem.OnComboChanged -= HandleComboChange;
        _comboSystem.OnComboReset -= HandleComboReset;
    }

    private void ResetVariables()
    {
        _timeDelay = 0f;
        _isDropSlime = false;
        _trigerRemoveSlime = false;
        _canPlay = true;
        IsGameOver = false;
    }

    private void waitToSpawn(float t = 3f)
    {
        if(_timeDelay < t)
        { 
            _timeDelay += Time.deltaTime;
            return;
        }
        if(_spawnSystem.SlimeHolder != null &&
        _spawnSystem.SlimeHolder.transform.parent == null) return;
        _spawnSystem._canSpawn = true;
        _isDropSlime = false;
        _timeDelay = 0;
    }

    private void DropSlime()
    {
        if(_spawnSystem.SlimeHolder == null) return;
        _spawnSystem.SlimeHolder.Unfreeze();
        MoveSlimeToPitContent(_spawnSystem.SlimeHolder);
        _spawnSystem.EmptyHolder();
        _isDropSlime = true;
    }

    private void MoveSlimeToPitContent(Slime slime)
    {
        Collider2D coll = slime.GetComponent<Collider2D>();
        _pitCtrl.AddToPit(slime.gameObject);
    }

    private void DragSlime_X()
    {
        if(_spawnSystem.SlimeHolder == null) return;
        Transform slimeobj = _spawnSystem.SlimeHolder.transform;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 slimePos = slimeobj.position;
        Vector3 dragVector = mousePos - slimePos;
        if(dragVector.magnitude > 0.1f)
        {
            Vector3 dragDirection = dragVector.normalized;
            Vector2 pos = mousePos;   
            pos.x = Mathf.Clamp(pos.x,-_dragThreshold,_dragThreshold);
            pos.y = slimeobj.position.y;
            slimeobj.position = pos;
        }
    }

    private void CheckGameOverByTimeout(float t)
    {
        if(_timeDelay < t)
        {
            _timeDelay += Time.deltaTime;
            return;
        }
        HandleGameOver();
    }

    private void HandleGameOver()
    {
        IsGameOver = true;
        _gameManager.ShowGameOverHud();
        PausePlay();
        _scoreSystem.SetScore(0);
    }
# region====>Support Actions<====
    public void ReviveSupport()
    {
        _reviveAction.OnAction();
        ResetVariables();
        waitToSpawn(0f);
    }
    public void TrigerRemoveSlimesSupport()
    {
        _trigerRemoveSlime = true;
    }
    public void RemoveSlimesSupport()
    {
        _removeSlimeAction.OnAction();
        _trigerRemoveSlime = false;
    }
#endregion
    public void CalScoreByLevel(int lv)
    {
        _comboSystem.AddComboCount();

        int score = ((lv+1)*(lv+2))/2;
        score = score * _comboSystem.ComBoCount;
        _scoreSystem.AddScore(score);
    }
    private void HandleComboChange(int cb)
    {
        _CombowindowCtrl.SetCombo(cb);
        _CombowindowCtrl.show();
    }
    private void HandleComboReset() => _CombowindowCtrl.Hide();

    void OnDrawGizmosSelected()
    {
        if(_pitCtrl == null) return;
        Gizmos.color = Color.red;
        CompositeCollider2D pitCollider = _pitCtrl.GetComponent<CompositeCollider2D>();
        Vector2 pitPos = pitCollider.bounds.center;
        pitPos.y = pitCollider.bounds.max.y;
        Gizmos.DrawWireCube(pitPos,new Vector3(_dragThreshold*2, 0,0));
    }
}