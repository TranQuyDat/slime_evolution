using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

class GamePlay : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem ;
    [SerializeField] private SpawnSystem _spawnSystem;
    [SerializeField] private GameObject _pitPrefab;
    [SerializeField] private float _dragThreshold = 0.5f;

    private GameManager _gameManager ;
    private PitController _pitCtrl;
    private float _timeDelay = 0f;
    public bool IsGameOver {get;private set;}
    private bool _isDropSlime;
    private bool _canPlay;
    void Awake()
    {
        _gameManager = GameManager.Instance;
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

        if(_pitCtrl.HadOverflowed)
        {
            CheckGameOverByTimeout(3f);
            return;
        }

        
        if(!_spawnSystem._canSpawn && _isDropSlime)
            waitToSpawn(3f);

           
    }

    public void StartPlay()
    {
        if(_pitCtrl == null)
        {
            // create pit
            GameObject pitObj = Instantiate(_pitPrefab,transform); 
            _pitCtrl = pitObj.GetComponent<PitController>();

            CompositeCollider2D compositeCol = pitObj.GetComponent<CompositeCollider2D>();
            float pitSizeY = compositeCol.bounds.size.y/2f;
            Vector2 pos =  Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.1f,10f));
            pos.y += pitSizeY;
            pitObj.transform.position = pos; // set pos for pit
        }
        
        _pitCtrl.gameObject.SetActive(true);

        _spawnSystem._canSpawn = true;
        IsGameOver = false;
        _timeDelay = 0f;
        _canPlay = true;
        waitToSpawn(0f);

    }
    public void PausePlay() => _canPlay = false;
    public void ResumePlay() => _canPlay = true;
    
    public void ResetPlay()
    {
        IsGameOver = false;
        _spawnSystem.Reset();
        _pitCtrl.ClearAllContent();
        _timeDelay = 0f;
        _canPlay = true;
        waitToSpawn(0f);
    }

    public void StopAndClearPlay()
    {
        _canPlay = false;
        _spawnSystem.Reset();
        _pitCtrl.ClearAllContent();
        _timeDelay = 0f;
        _pitCtrl.gameObject.SetActive(false);
    }


    private void waitToSpawn(float t = 3f)
    {
        if(_timeDelay < t)
        { 
            _timeDelay += Time.deltaTime;
            return;
        }
        _spawnSystem._canSpawn = true;
        _isDropSlime = false;
        _timeDelay = 0;
    }

    private void DropSlime()
    {
        if(_spawnSystem.SlimeHolder == null) return;
        _spawnSystem.SlimeHolder.Unfreeze();
        StartCoroutine(MoveSlimeToPitContent(_spawnSystem.SlimeHolder));
        _spawnSystem.EmptyHolder();
        _isDropSlime = true;
    }

    IEnumerator MoveSlimeToPitContent(Slime slime)
    {
        Collider2D coll = slime.GetComponent<Collider2D>();
        yield return new WaitUntil(() => (_pitCtrl.TopYpit > coll.bounds.min.y));
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
        _canPlay = false;
        IsGameOver = true;
        _gameManager.ShowGameOverHud();
        PausePlay();
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
}