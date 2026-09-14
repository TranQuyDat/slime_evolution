using System;
using UnityEngine;
[Serializable]
public struct SlimeData 
{
    [SerializeField] private int _lv;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _scale;
    [Header("Collider")]
    [SerializeField] private Vector2 _colliderSize;
    [SerializeField] private Vector2 _colliderOffset;
    [SerializeField] private CapsuleDirection2D _colliderDirection;

    public SlimeData(int lv , Sprite sprite,float scale)
    {
        _lv = lv;
        _sprite = sprite;
        _scale = scale;
        _colliderSize = sprite != null ? sprite.bounds.size : Vector2.one;
        _colliderOffset = sprite != null ? sprite.bounds.center : Vector2.zero;
        _colliderDirection = _colliderSize.x >= _colliderSize.y
            ? CapsuleDirection2D.Horizontal
            : CapsuleDirection2D.Vertical;
    }
    public int Lv => _lv;
    public Sprite Sprite => _sprite;
    public float Scale => _scale;
    public Vector2 ColliderSize => _colliderSize;
    public Vector2 ColliderOffset => _colliderOffset;
    public CapsuleDirection2D ColliderDirection => _colliderDirection;
    public bool HasColliderData => _colliderSize.x > 0f && _colliderSize.y > 0f;
}
