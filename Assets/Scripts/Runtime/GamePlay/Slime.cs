using System;
using System.Threading.Tasks;
using UnityEngine;

class Slime : MonoBehaviour ,IPoolable,IDestroyable
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private CapsuleCollider2D _collider;
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
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();
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
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _collider.enabled = true;
        _sr.sprite = data.Sprite;
        FitColliderToSprite();
        scaleSlime(data.Scale);
        _originScale = transform.localScale;
    }

    private void FitColliderToSprite()
    {
        if (_sr.sprite == null || _collider == null) return;

        Bounds bounds = _sr.sprite.bounds;
        Vector2 size = bounds.size;
        _collider.offset = bounds.center;
        _collider.size = new Vector2(
            Mathf.Max(size.x, 0.01f),
            Mathf.Max(size.y, 0.01f));
        _collider.direction = size.x >= size.y
            ? CapsuleDirection2D.Horizontal
            : CapsuleDirection2D.Vertical;
    }

    void OnEnable()
    {
        GameEvents.OnDragonExploded += HandleDragonExploded;
        _isDestroying = false;
    }

    private void Squash_Stretch()
    {
        if (_isDestroying) return;

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
        // Slime đang được giữ nằm dưới GamePlay, không nằm trong PitController.
        // Chỉ slime đã được thả vào nồi mới chịu ảnh hưởng của vụ nổ.
        if (_isDestroying || GetComponentInParent<PitController>() == null)
            return;

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
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
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
