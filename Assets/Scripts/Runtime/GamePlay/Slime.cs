using System;
using System.Threading.Tasks;
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
    public bool IsTouching => _rb.IsTouchingLayers(LayerMask.GetMask("Slime"));
    public string PoolKey =>"Slime";
    public Material Material => _sr.material;
    public SpriteRenderer Sr => _sr;
    public SlimeVisual Visual {get; private set;}
    public bool IsDestroying => _isDestroying;
    public SlimeMerge SlimeMerge {get; private set;}
    public Collider2D Collider => _collider;

    private BaseAudioEvent _collisionAudioEvent;
    private Vector3 _originScale;
    private Delay _delay = new();
    private bool _isInPit; 
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        Visual = GetComponent<SlimeVisual>();
        SlimeMerge = GetComponent<SlimeMerge>();
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
        var pit  = GetComponentInParent<PitController>();
        _isInPit = pit !=null;
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

    private async void HandleDragonExploded(Slime dragon,Action<int> addScore)
    {
        if(_isDestroying || _isInPit) return;

        _isDestroying = true; 
        if(dragon == this)
        {
            //dragon vfx explosion
            Visual.PlayExplosion(0.8f,() =>
            {
                Destroy();
            });
            return;
        }
        await _delay.WaitSeconds(0.8f);
        addScore.Invoke(_data.Lv);
        Visual.PlayScoreCollectEffect(Destroy);
        
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