using UnityEngine;

class Slime : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private SlimeData _data;
    private bool _isFreeze;



    public bool IsFreeZe => _isFreeze;
    public SlimeData Data => _data;
    public bool IsTouching => _rb.IsTouchingLayers(LayerMask.GetMask("Slime","Ground"));

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity, 5f);
        
    }

    public void Init(SlimeData data)
    {
        _data = data;
        scaleSlime(data.Lv);
    }

    private void scaleSlime(int lv)
    {
        float scale = (1 + (lv - 1)) * 0.5f;
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