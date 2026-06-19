using System;
using UnityEngine;

public class Slime : MonoBehaviour ,IPoolable,IDestroyable
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Collider2D _collider;
    private SlimeData _data;
    private bool _isFreeze;
    private bool _isDestroying;

    [SerializeField] private int DisPLayLevel;

    public bool IsFreeZe => _isFreeze;
    public SlimeData Data => _data;
    public bool IsTouching => _rb.IsTouchingLayers(LayerMask.GetMask("Slime","Ground"));

    public string PoolKey =>"Slime";

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }
    void Update()
    {
    }
    void LateUpdate()
    {
        DisPLayLevel = _data.Lv;
    }

    public void Init(SlimeData data)
    {
        _data = data;
        _sr.sprite = data.Sprite;
        scaleSlime(data.Scale);
    }

    void OnEnable()
    {
        GameEvents.OnDragonExploded += HandleDragonExploded;
        _isDestroying = false;
    }

    private void HandleDragonExploded(Slime dragon)
    {
        if(_isDestroying) return;

        _isDestroying = true; 
        if(dragon == this)
        {
            //dragon vfx explosion

            //
            Destroy();
            return;
        }
        //vfx

        //
        Destroy();
    }

    private void scaleSlime(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public void Unfreeze()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _isFreeze = false;
    }
    public void Freeze()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _isFreeze = true;
    }

    public void Destroy()
    {
        _isDestroying = true;
        GameEvents.OnDragonExploded -= HandleDragonExploded;
        ObjectPoolSystem.Instance.Cancel(gameObject,PoolKey);
    }
}