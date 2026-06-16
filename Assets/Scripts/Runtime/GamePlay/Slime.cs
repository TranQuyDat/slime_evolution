using System;
using UnityEngine;

class Slime : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Collider2D _collider;
    private SlimeData _data;
    private bool _isFreeze;

    [SerializeField] private int DisPLayLevel;

    public bool IsFreeZe => _isFreeze;
    public SlimeData Data => _data;
    public bool IsTouching => _rb.IsTouchingLayers(LayerMask.GetMask("Slime","Ground"));

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }
    void Update()
    {
        // Vector3 velocity = _rb.linearVelocity;
        // velocity.y = Mathf.Clamp(_rb.linearVelocity.y, -10,10f);
        // _rb.linearVelocity = velocity;
        
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

}