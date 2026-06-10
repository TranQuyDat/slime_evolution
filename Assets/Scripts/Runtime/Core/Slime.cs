using UnityEngine;

class Slime : MonoBehaviour
{
    private SpriteRenderer _sr;
    private SlimeData _data;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
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

}