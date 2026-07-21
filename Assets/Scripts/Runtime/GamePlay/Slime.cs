using System;
using UnityEngine;

class Slime : MonoBehaviour ,IPoolable,IDestroyable
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
    public Material Material => _sr.material;
    public SlimeVisual Visual {get; private set;}
    private BaseAudioEvent _collisionAudioEvent;
    private Vector3 _originScale;
    private Delay _delay = new();
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        Visual = GetComponent<SlimeVisual>();
        _collisionAudioEvent = Resources.Load<BaseAudioEvent>("Events/Collision_Audio_Event");
        
    }
    void Update()
    {
        Squash_Stretch();
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
        _originScale = transform.localScale;
    }

    void OnEnable()
    {
        GameEvents.OnDragonExploded += HandleDragonExploded;
        _isDestroying = false;
    }

    private void Squash_Stretch()
    {
        if(_rb.linearVelocity.y > 0.2f)
        {
            float v = _rb.linearVelocity.magnitude;
            Visual.PlaySquash(v,_originScale);
        }
        if(_rb.linearVelocity.y < -5f)
        {
            float v = _rb.linearVelocity.magnitude;
            Visual.PlayStretch(v,_originScale);
        }
    }

    private void HandleDragonExploded(Slime dragon,Action<int> addScore)
    {
        if(_isDestroying) return;

        _isDestroying = true; 
        if(dragon == this)
        {
            //dragon vfx explosion

            //
            _delay.WaitSeconds(0.08f);
            Destroy();
            return;
        }
        //vfx

        //
        _delay.WaitSeconds(0.08f);
        addScore.Invoke(_data.Lv);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < 2f)
        return;

        // Va chạm với mặt đất
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _collisionAudioEvent.Play();
            return;
        }

        // Va chạm với slime
        if (collision.gameObject.layer != LayerMask.NameToLayer("Slime"))
            return;

        if (GetInstanceID() > collision.gameObject.GetInstanceID())
            return;

        SlimeMerge slimeMerge = collision.gameObject.GetComponent<SlimeMerge>();
        if (slimeMerge.IsMerging)
            return;

        _collisionAudioEvent.Play();
    
    }

    public void Destroy()
    {
        _isDestroying = true;
        GameEvents.OnDragonExploded -= HandleDragonExploded;
        ObjectPoolSystem.Instance.Cancel<Slime>(this,PoolKey);
    }
}