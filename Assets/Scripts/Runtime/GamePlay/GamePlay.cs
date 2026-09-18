using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

class GamePlay : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem ;
    [SerializeField] private SlimeSpawnManager _slimeSpawn;
    [SerializeField] private GameObject _pitPrefab;
    [SerializeField] private ComboWindow _comboWindowPrefab;
    [SerializeField] private float _dragThreshold = 0.5f;
    [SerializeField] private float _dragSmoothTime = 0.04f;

    private GameManager _gameManager;
    private ScoreSystem _scoreSystem;
    private ComboSystem _comboSystem;
    private PitController _pitCtrl;
    private float _timeDelay = 0f;
    private float _spawnReadyTime;
    private bool _isSpawnTimerRunning;
    private float _dragVelocityX;
    private bool _CanDropSlime;
    private bool _canControlSpawnedSlime;
    private bool _isSpawnPopupPlaying;
    private bool _canPlay;
    private bool _isSpawning;
    private bool _trigerRemoveSlime;
    private SupportAction _reviveAction;
    private RemoveSlimeAction _removeSlimeAction;
    private Camera _camera;
    private CameraShake _cametaShake;
    private Slime _slimeHolder;
    private int _highestUnlockedLevel = 0;
    private int _highestPitLevelValue;
    private int _spawnRequestVersion;
    public bool IsGameOver {get;private set;}
    public ScoreSystem ScoreSystem => _scoreSystem;

#region Initialize
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _scoreSystem = new ScoreSystem(); 
        _comboSystem = new ComboSystem();
        _camera = Camera.main;
        _cametaShake = _camera.GetComponent<CameraShake>();
       
    }
    void Start()
    {
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
        Slime slimeAbove = _pitCtrl.GetSlimeAbove();
        bool hasOverflowed = slimeAbove != null &&
            slimeAbove.Collider.bounds.max.y > _pitCtrl.TopYpit;
        if(hasOverflowed && !slimeAbove.SlimeMerge.IsMerging)
        {
            if(!_cametaShake.IsShaking)
                _cametaShake.Shake(1.5f,0.2f);
            CheckGameOverByTimeout(3f);
            return;
        }

        // Chỉ Game Over khi slime nằm trên ngưỡng liên tục đủ thời gian.
        _timeDelay = 0f;
        
        _comboSystem.ResetComboByTime(1.5f);
        if(_slimeHolder != null) return;
        waitToSpawn(1f);
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
            Vector2 pos =  _camera.ViewportToWorldPoint(new Vector3(0.5f,0.1f,10f));
            pos.y += pitSizeY;
            pitObj.transform.position = pos; // set pos for pit
            Physics2D.SyncTransforms();
        }
        _pitCtrl.gameObject.SetActive(true);
    }
    public void InitializeUI()
    {
        _scoreSystem.SetScore(0);
        ResetProgress();
    }
    public void InitializeSupport()
    {
        if(_reviveAction == null)
            _reviveAction = new ReviveAction(_pitCtrl);
        
        if(_removeSlimeAction == null)
            _removeSlimeAction = new RemoveSlimeAction(_inputSystem);
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
        RefreshPreview();
        _canPlay = false;
        _pitCtrl.FxShowPit().OnComplete(() =>
        {
            _canPlay = true;
            waitToSpawn(0f);
        });
    }
    public void PausePlay()
    {
        _canPlay = false;
    } 
    public void ResumePlay()
    {
        _canPlay = true;
        RefreshPreview();
    } 
    
    public void ResetPlay()
    {
        IsGameOver = false;
        InitializeUI();
        clearRound();
        ResetVariables();
        RefreshPreview();
        waitToSpawn(0f);
        _removeSlimeAction.OnFinish();
    }

    public void StopAndClearPlay()
    {
        _spawnRequestVersion++;
        _canPlay = false;
        _timeDelay = 0f;
        _spawnReadyTime = 0f;
        _isSpawnTimerRunning = false;
        _pitCtrl.gameObject.SetActive(false);
        clearRound();
        _comboSystem.OnComboChanged -= HandleComboChange;
    }
    #endregion

#region Spawn
    private void waitToSpawn(float t = 3f)
    {
        if (_isSpawning) return;

        if (!_isSpawnTimerRunning)
        {
            _spawnReadyTime = Time.time + Mathf.Max(0f, t);
            _isSpawnTimerRunning = true;
        }

        if (Time.time < _spawnReadyTime) return;

        if(_slimeHolder != null &&
        _slimeHolder.transform.parent == null) return;

        _isSpawnTimerRunning = false;
        _isSpawning = true;
        int requestVersion = _spawnRequestVersion;
        Vector3 spawnPosition = GetPointerSpawnPosition();
        SlimeData spawnData = _slimeSpawn.TakeNextSlimeData();

        _gameManager.FlyPreviewToSpawn(
            spawnPosition,
            spawnData.Sprite,
            spawnData.Scale,
            () =>
            {
                if (requestVersion != _spawnRequestVersion)
                {
                    _isSpawning = false;
                    return;
                }

                _slimeHolder = _slimeSpawn.Spawn(spawnData, spawnPosition);
                _slimeHolder.transform.SetParent(transform,true);
                _dragVelocityX = 0f;
                _canControlSpawnedSlime = true;
                _isSpawnPopupPlaying = true;
                Slime spawnedSlime = _slimeHolder;
                spawnedSlime.Visual.PlaySpawnEffect(() =>
                {
                    if (_slimeHolder == spawnedSlime)
                        _isSpawnPopupPlaying = false;
                });

                Sprite nextSprite = _slimeSpawn.PreviewNextSlime().Sprite;
                _gameManager.UpdatePreviewHud(nextSprite);

                _timeDelay = 0f;
                _isSpawning = false;
            });
    }

    private void MoveSlimeToPitContent(Slime slime)
    {
        _slimeHolder = null;
        _pitCtrl.AddToPit(slime.gameObject);
        UpdateProgress(slime.Data.Lv);
    }

    private void RefreshPreview()
    {
        _gameManager.UpdatePreviewHud(_slimeSpawn.PreviewNextSlime().Sprite);
    }

    private Vector3 GetPointerSpawnPosition()
    {
        Vector3 position = _slimeSpawn.GetSpawnPosition();
        if (_inputSystem.TryGetPointerPosition(out Vector2 pointerPosition))
        {
            float pointerX = _camera.ScreenToWorldPoint(pointerPosition).x;
            position.x = Mathf.Clamp(pointerX, -_dragThreshold, _dragThreshold);
        }
        return position;
    }

    #endregion

#region Input

    public void HandlePrimaryInput()
    {
        if (_trigerRemoveSlime)
            _removeSlimeAction.HandleInput();
        else
            DropSlime();
    }

    private void DropSlime()
    {
        if(_inputSystem.IsPointerOverUI() || !_CanDropSlime || !_canControlSpawnedSlime
        || _isSpawnPopupPlaying
        || _slimeHolder == null || !_canPlay) return;
        _slimeHolder.Unfreeze();
        MoveSlimeToPitContent(_slimeHolder);
    }

    private void DragSlime_X()
    {
        if(_slimeHolder == null || !_canControlSpawnedSlime) return;
        if (!_inputSystem.TryGetPointerPosition(out Vector2 pointerPosition)) return;

        Vector3 mousePos = _camera.ScreenToWorldPoint(pointerPosition);
        Vector3 slimePos = _slimeHolder.transform.position;
        float targetX = Mathf.Clamp(
            mousePos.x,
            -_dragThreshold,
            _dragThreshold);

        if (Mathf.Abs(targetX - slimePos.x) <= 0.001f) return;

        slimePos.x = _dragSmoothTime <= 0f
            ? targetX
            : Mathf.SmoothDamp(
                slimePos.x,
                targetX,
                ref _dragVelocityX,
                _dragSmoothTime);
        _slimeHolder.transform.position = slimePos;
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
        _reviveAction.OnAction(RefreshProgressFromPit);
        ResetVariables();

        if (_slimeHolder != null && _slimeHolder.gameObject.activeInHierarchy)
        {
            _slimeHolder.Freeze();
            _canControlSpawnedSlime = true;
            _isSpawnPopupPlaying = false;
            return;
        }

        _slimeHolder = null;
        waitToSpawn(0f);
    }
    public void TrigerRemoveSlimesSupport()
    {
        _trigerRemoveSlime = true;
        _CanDropSlime = false;
        _removeSlimeAction.OnEnter();
    }
    public void RemoveSlimesSupport(Action Oncomplete = null)
    {
        _removeSlimeAction.OnAction(() =>
        {
            RefreshProgressFromPit();
            _trigerRemoveSlime = false;
            _CanDropSlime = true;
            Oncomplete?.Invoke();
        });
    }
    public void CancleSlimeSupport()
    {
        _removeSlimeAction.OnFinish();
        _trigerRemoveSlime = false;
        _CanDropSlime = true;
    }
#endregion
 
#region  sub Method
    private void ResetVariables()
    {
        _spawnRequestVersion++;
        _timeDelay = 0f;
        _spawnReadyTime = 0f;
        _isSpawnTimerRunning = false;
        _dragVelocityX = 0f;
        _CanDropSlime = true;
        _canControlSpawnedSlime = false;
        _isSpawning = false;
        _trigerRemoveSlime = false;
        _canPlay = true;
        IsGameOver = false;
        _highestUnlockedLevel = 0;
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
    public void OnSlimeMerged(int newLevel)
    {
        UpdateProgress(newLevel);
        _highestUnlockedLevel = Mathf.Max(_highestUnlockedLevel, newLevel);
        if (_slimeSpawn.SwapDeck(_highestUnlockedLevel))
            RefreshPreview();
    }

    private void UpdateProgress(int zeroBasedLevel)
    {
        int value = zeroBasedLevel + 1;
        if (value <= _highestPitLevelValue) return;

        _highestPitLevelValue = value;
        _gameManager.UpdateProgressHud(value);
    }

    private void RefreshProgressFromPit()
    {
        int highestValue = 0;
        Slime[] slimes = _pitCtrl.GetAllContents<Slime>();

        for (int i = 0; i < slimes.Length; i++)
        {
            Slime slime = slimes[i];
            if (slime == null || slime.IsDestroying) continue;
            highestValue = Mathf.Max(highestValue, slime.Data.Lv + 1);
        }

        if (highestValue == _highestPitLevelValue) return;
        _highestPitLevelValue = highestValue;
        _gameManager.UpdateProgressHud(highestValue);
    }

    private void ResetProgress()
    {
        _highestPitLevelValue = 0;
        _gameManager.UpdateProgressHud(0);
    }

    public Slime[] GetSlimesInPit()
    {
        return _pitCtrl == null
            ? Array.Empty<Slime>()
            : _pitCtrl.GetAllContents<Slime>();
    }

    public void CalScoreByLevel(int lv,Vector2 pos)
    {
        _comboSystem.AddComboCount();

        int score = ((lv+1)*(lv+2))/2;
        score = score * _comboSystem.ComBoCount;
        _scoreSystem.AddScore(score);
        _gameManager.RunFloatingScore((score,pos));
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
