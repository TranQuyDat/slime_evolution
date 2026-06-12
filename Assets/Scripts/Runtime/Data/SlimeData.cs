using UnityEngine;

public class SlimeData 
{
    [SerializeField] private int _lv;
    public SlimeData(int lv)
    {
        _lv = lv;
    }
    public int Lv => _lv;
}
