using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

class GamePlay : MonoBehaviour
{
    private GameManager _gameManager = GameManager.Instance;
    [SerializeField] private InputSystem _inputSystem ;
    [SerializeField] private SpawnSystem _spawnSystem;
    [SerializeField] private GameObject _pitPrefab;
    [SerializeField] private float _dragThreshold = 0.5f;
    private PitController _pitCtrl;
    private float _timeDelay = 0f;
    private bool isDropSlime;
    void Start()
    {
        _inputSystem.BindAction(KeyCode.Mouse0,DropSlime);
        _spawnSystem._canSpawn = true;
        isDropSlime = false;
        Init();
    }

    void Update()
    {
        DragSlime_X();

        if(_pitCtrl.HasOverflowed)
        {
            CheckGameOver(3f);
            return;
        }

        
        if(!_spawnSystem._canSpawn && isDropSlime)
            waitToSpawn(3f);
           
    }

    private void Init()
    {
        GameObject pitObj = Instantiate(_pitPrefab,transform); 
        CompositeCollider2D compositeCol = pitObj.GetComponent<CompositeCollider2D>();
        _pitCtrl = pitObj.GetComponent<PitController>();
        float pitSizeY = compositeCol.bounds.size.y/2f;
        Vector2 pos =  Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.1f,10f));
        pos.y += pitSizeY;
        _pitCtrl.transform.position = pos;
    }

    private void waitToSpawn(float t = 3f)
    {
        if(_timeDelay < t)
        { 
            _timeDelay += Time.deltaTime;
            return;
        }
        _spawnSystem._canSpawn = true;
        isDropSlime = false;
        _timeDelay = 0;
    }

    private void DropSlime()
    {
        if(_spawnSystem.SlimeHolder == null) return;
        _spawnSystem.SlimeHolder.Unfreeze();
        StartCoroutine(MoveSlimeToPitContent(_spawnSystem.SlimeHolder));
        _spawnSystem.EmptyHolder();
        isDropSlime = true;
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

    private void CheckGameOver(float t)
    {
        if(_timeDelay < t)
        {
            _timeDelay += Time.deltaTime;
            return;
        }
        _spawnSystem.Reset();
        _pitCtrl.ClearAllContent();
        _timeDelay = 0f;
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