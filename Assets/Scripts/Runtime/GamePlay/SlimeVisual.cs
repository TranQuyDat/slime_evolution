using DG.Tweening;
using UnityEngine;

class SlimeVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slime _slime;
    private BaseVfxEvent _mergeVfxEvent;

    private Sequence _seq;
    void Awake()
    {
        
        _mergeVfxEvent =Resources.Load<BaseVfxEvent>("Events/Merge_Vfx_Event");
    }
    void OnDisable()
    {
        _seq?.Kill();
    }
    public void PlayMergeEffect()
    {
        _seq?.Kill();

        _seq = DOTween.Sequence();
        _slime.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        _seq.Append(_slime.transform.DOScale(_slime.Data.Scale,0.12f)
        .SetEase(Ease.OutBack));
        
        _seq.OnStart(() =>
        {
            _mergeVfxEvent.Play(new()
            {
            Position = transform.TransformPoint(new Vector3(0,0.5f,0)),
            Speed = 1f,
            Scale = Vector2.one* _slime.Data.Scale,
            });
        });

    }
    public void PlayDestroyEffect()
    {
        _seq?.Kill();

        
    }
    public void PlaySpawnEffect()
    {
        _seq?.Kill();

        
    }
}