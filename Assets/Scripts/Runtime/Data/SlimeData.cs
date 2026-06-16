using System;
using UnityEngine;
[Serializable]
public struct SlimeData 
{
    [SerializeField] private int _lv;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _scale;
    public SlimeData(int lv , Sprite sprite,float scale)
    {
        _lv = lv;
        _sprite = sprite;
        _scale = scale;
    }
    public int Lv => _lv;
    public Sprite Sprite => _sprite;

    public float Scale => _scale;
}
