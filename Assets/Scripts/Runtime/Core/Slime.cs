using UnityEngine;
public enum Slimestate
{
    Holding,
    Falling,
    Landed
}
class Slime : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private SlimeData _data;

    public Slimestate _curstate = Slimestate.Holding;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }
    void Update()
    {
        _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity, 5f);
        
    }

    public void Init(SlimeData data)
    {
        _data = data;
        _sr.color = data.Color;
        scaleSlime((int)data.Lv);
    }

    private void scaleSlime(int lv)
    {
        float scale = (1 + (lv - 1)) * 0.25f;
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public void Unfreeze()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

}