using UnityEngine;

public class SlimeData 
{
    [SerializeField] private int _lv;
    [SerializeField] private Color _color;
    public SlimeData(int lv) : this(lv,Color.white){}
    public SlimeData(int lv, Color color)
    {
        _lv = lv;
        _color = color;
    }
    public int Lv => _lv;
    public Color Color => _color;
}
