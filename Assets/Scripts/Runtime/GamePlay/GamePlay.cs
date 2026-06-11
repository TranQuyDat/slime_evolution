using System;
using UnityEngine;

class GamePlay : MonoBehaviour
{
    private GameManager _gameManager = GameManager.Instance;
    [SerializeField] private InputSystem _inputSystem ;
    [SerializeField] private SpawnSystem _spawnSystem;
    [SerializeField] private Transform _Pit;
    [SerializeField] private float _dragThreshold = 0.5f;
    void Start()
    {
        _inputSystem.BindAction(KeyCode.Mouse0,dropSlime);
        _spawnSystem._canSpawn = true;
    }

    void Update()
    {
        DragSlime_X();
    }

    private void dropSlime()
    {
        if(_spawnSystem.SlimeHolder == null) return;
        _spawnSystem.SlimeHolder.Unfreeze();
        _spawnSystem._canSpawn = true;
    }

    private void DragSlime_X()
    {
        Transform slimeobj = _spawnSystem.SlimeHolder.transform;
        if(slimeobj == null) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 slimePos = slimeobj.position;
        Vector3 dragVector = mousePos - slimePos;
        if(dragVector.magnitude > _dragThreshold)
        {
            Vector3 dragDirection = dragVector.normalized;
            Vector2 pos = slimeobj.position + dragDirection * Time.deltaTime * 5f;   
            pos.x = Mathf.Clamp(pos.x,-_dragThreshold,_dragThreshold);
            pos.y = slimeobj.position.y;
            slimeobj.position = pos;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        CompositeCollider2D pitCollider = _Pit.GetComponent<CompositeCollider2D>();
        Vector2 pitPos = pitCollider.bounds.center;
        pitPos.y = pitCollider.bounds.max.y;
        Gizmos.DrawWireCube(pitPos,new Vector3(_dragThreshold*2, 0,0));
    }
}