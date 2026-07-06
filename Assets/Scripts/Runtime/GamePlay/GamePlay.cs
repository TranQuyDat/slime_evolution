using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

class GamePlay : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem ;
    [SerializeField] private SlimeSpawnManager _slimeSpawn;
    [SerializeField] private GameObject _pitPrefab;
    [SerializeField] private ComboWindow _comboWindowPrefab;
    [SerializeField] private float _dragThreshold = 0.5f;

    private GameManager _gameManager;
    private ScoreSystem _scoreSystem;
    private ComboSystem _comboSystem;
    private PitController _pitCtrl;
    private float _timeDelay = 0f;
    private bool _CanDropSlime;
    private bool _canPlay;
    private bool _trigerRemoveSlime;
    private SupportAction _reviveAction;
    private SupportAction _removeSlimeAction;
    private Camera _camera;
    private Slime _slimeHolder;
    public bool IsGameOver {get;private set;}
    public ScoreSystem ScoreSystem => _scoreSystem;

#region Initialize
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _scoreSystem = new ScoreSystem(); 
        _comboSystem = new ComboSystem();
        _camera = Camera.main;
       
    }
    void Start()
    {
        _inputSystem.BindAction(KeyCode.Mouse0,DropSlime);
        _canPlay = false;
    }

    void Update()
    {
        if(!_canPlay) return;

        if(_trigerRemoveSlime)
        {
            _removeSlimeAction.OnUpdate();
            return;
        }

        DragSlime_X();
        if(_pitCtrl.HadOverflowed)
        {
            CheckGameOverByTimeout(3f);
            return;
        }
        
        _comboSystem.ResetComboByTime(1.5f);
        if(_slimeHolder != null) return;
        waitToSpawn(3f);
    }
    public void InitializePit()
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
        _pitCtrl.gameObject.SetActive(true);
    }
    public void InitializeUI()
    {
        _scoreSystem.SetScore(0);
    }
    public void InitializeSupport()
    {
        if(_reviveAction == null)
            _reviveAction = new ReviveAction(_pitCtrl);
        
        if(_removeSlimeAction == null)
            _removeSlimeAction = new RemoveSlimeAction(_pitCtrl,_inputSystem);
    }
    public void SubscribeEvents()
    {
        _comboSystem.OnComboChanged += HandleComboChange;
    }
    #endregion

#region Gameplay    
    public void BeginRound()
    {
        InitializePit();
        InitializeUI();
        InitializeSupport();
        SubscribeEvents();
        ResetVariables();
    }
    public void StartPlay()
    {
        BeginRound();
        waitToSpawn(0f);    
    }
    public void PausePlay()
    {
        _canPlay = false;
        _CanDropSlime = false;
    } 
    public void ResumePlay()
    {
        _canPlay = true;
        _CanDropSlime = true;
    } 
    
    public void ResetPlay()
    {
        IsGameOver = false;
        _scoreSystem.SetScore(0);
        clearRound();
        ResetVariables();
        waitToSpawn(0f);
        _removeSlimeAction.OnFinish();
    }

    public void StopAndClearPlay()
    {
        _canPlay = false;
        _timeDelay = 0f;
        _pitCtrl.gameObject.SetActive(false);
        clearRound();
        _comboSystem.OnComboChanged -= HandleComboChange;
    }
    #endregion

#region Spawn
    private void waitToSpawn(float t = 3f)
    {
        if(_timeDelay < t)
        { 
            _timeDelay += Time.deltaTime;
            return;
        }
        if(_slimeHolder != null &&
        _slimeHolder.transform.parent == null) return;
        _slimeHolder = _slimeSpawn.Spawn();
        Sprite sprite = _slimeSpawn.PreviewNextSlime().Sprite;
        _gameManager.UpdatePreviewHud(sprite);
        _slimeHolder.transform.SetParent(transform,true);
        _timeDelay = 0;
    }

    private void MoveSlimeToPitContent(Slime slime)
    {
        _slimeHolder = null;
        _pitCtrl.AddToPit(slime.gameObject);
    }

    #endregion

#region Input

    private void DropSlime()
    {
        if(IsPointerOverUI() || !_CanDropSlime || _slimeHolder == null) return;
        _slimeHolder.Unfreeze();
        MoveSlimeToPitContent(_slimeHolder);
    }

    private void DragSlime_X()
    {
        if(_slimeHolder == null) return;
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 slimePos = _slimeHolder.transform.position;
        Vector3 dragVector = mousePos - slimePos;

        if(dragVector.magnitude <= 0.1f)return;
        
        Vector2 pos = mousePos;   
        pos.x = Mathf.Clamp(pos.x,-_dragThreshold,_dragThreshold);
        pos.y = _slimeHolder.transform.position.y;
        _slimeHolder.transform.position = pos;
        
    }
#endregion
   
#region GameOver
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
    }
    #endregion

#region Support Actions
    public void ReviveSupport()
    {
        _reviveAction.OnAction();
        ResetVariables();
        waitToSpawn(0f);
    }
    public void TrigerRemoveSlimesSupport()
    {
        _trigerRemoveSlime = true;
        _CanDropSlime = false;
        _removeSlimeAction.OnEnter();
    }
    public void RemoveSlimesSupport()
    {
        _removeSlimeAction.OnAction();
        _trigerRemoveSlime = false;
        _CanDropSlime = true;
    }
#endregion
 
#region  sub Method
    private void ResetVariables()
    {
        _timeDelay = 0f;
        _CanDropSlime = true;
        _trigerRemoveSlime = false;
        _canPlay = true;
        IsGameOver = false;
    }
    private void clearRound()
    {
        _slimeSpawn.Reset();
        _pitCtrl.ClearAllContent();

        if (_slimeHolder != null)
        {
            _slimeHolder.Destroy();
            _slimeHolder = null;
        }
    }
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }   
    public void CalScoreByLevel(int lv)
    {
        _comboSystem.AddComboCount();

        int score = ((lv+1)*(lv+2))/2;
        score = score * _comboSystem.ComBoCount;
        _scoreSystem.AddScore(score);
    }
    private void HandleComboChange(int cb)
    {
        _gameManager.updateComboHud(cb);
    }

    void OnDrawGizmosSelected()
    {
        if(_pitCtrl == null) return;
        Gizmos.color = Color.red;
        CompositeCollider2D pitCollider = _pitCtrl.GetComponent<CompositeCollider2D>();
        Vector2 pitPos = pitCollider.bounds.center;
        pitPos.y = pitCollider.bounds.max.y;
        Gizmos.DrawWireCube(pitPos,new Vector3(_dragThreshold*2, 0,0));
    }
    #endregion
}