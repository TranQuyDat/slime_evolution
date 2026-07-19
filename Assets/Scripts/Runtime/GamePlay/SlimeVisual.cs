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

    public void PlayStretch(float speed , Vector3 originScale)
    {
        if(speed < 2f) return;
        
        float strength = Mathf.InverseLerp(2f,12f,speed);
        float stretchy = Mathf.Lerp(originScale.x+0.05f,originScale.x+0.15f,strength);
        float stretchx = Mathf.Lerp(originScale.y-0.05f,originScale.x-0.15f,strength);

        _seq?.Kill();
        _seq = DOTween.Sequence();
        Tween tw1 = transform.DOScale(new Vector3(stretchx,stretchy,1),0.08f).SetEase(Ease.OutQuad);
        Tween tw2 = transform.DOScale(originScale,0.12f).SetEase(Ease.OutBack);
        _seq.Append(tw1);
        _seq.Append(tw2);

        
    }
    public void PlaySquash(float speed , Vector3 originScale)
    {
        if(speed < 2f) return;
        
        float strength = Mathf.InverseLerp(2f,12f,speed);
        float squashx = Mathf.Lerp(originScale.x+0.05f,originScale.x+0.15f,strength);
        float squashy = Mathf.Lerp(originScale.y-0.05f,originScale.x-0.15f,strength);

        _seq.Kill();
        _seq  = DOTween.Sequence();
        Tween tw1 = transform.DOScale(new Vector3(squashx,squashy,1),0.08f).SetEase(Ease.OutQuad);
        Tween tw2 = transform.DOScale(originScale,0.12f).SetEase(Ease.OutBack);
        _seq.Append(tw1);
        _seq.Append(tw2);
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