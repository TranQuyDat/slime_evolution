using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

class SlimeVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slime _slime;
    private SimpleVfxEvent _mergeVfxEvent;
    private SimpleVfxEvent _explosionVfxEvent;
    private TargetMoveVfxEvent _slimeScoreCollectVfxEvent;
    

    private Sequence _seq;
    void Awake()
    {
        _mergeVfxEvent =Resources.Load<SimpleVfxEvent>("Events/Merge_Vfx_Event");
        _explosionVfxEvent =Resources.Load<SimpleVfxEvent>("Events/Explosion_Vfx_Event");
        _slimeScoreCollectVfxEvent = Resources.Load<TargetMoveVfxEvent>("Events/Slime_Score_Collect_Vfx_Event");
    }
    void OnDisable()
    {
        _seq?.Kill();
    }
    [ContextMenu("TestPlayExplosion")]
    public void PlayExplosion(float duration,Action oncomplete = null)
    {
        transform.DOShakePosition(
            duration: duration,
            strength: 0.08f,
            vibrato: 15,
            randomness: 60f,
            fadeOut: false
        ).OnComplete(() =>
        {
            Vector3 pos = transform.TransformPoint(new Vector3(0,0.5f,0));
            _explosionVfxEvent.Play(new()
            {
                Position = pos
            });
            oncomplete?.Invoke();
        });
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

    [ContextMenu("TestPlayScoreCollectEffect")]
    public void TestPlayScoreCollectEffect()
    {
        PlayScoreCollectEffect(_slime.Destroy);
    }
    public void PlayScoreCollectEffect(Action onComplete = null)
    {
        transform.DOKill();

        Vector3 originScale = transform.localScale;

        SpriteRenderer sr = _slime.Sr;

        DOTween.Sequence()
            .Append(
                transform.DOScale(originScale * 1.5f, 0.35f)
                    .SetEase(Ease.OutSine)
            )
            .OnComplete(() =>
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
                transform.localScale = originScale;
                // vfx bụi phép bay 
                _slimeScoreCollectVfxEvent.Play(new()
                {
                    Position = transform.position,
                    Speed = 1f,
                    Scale = Vector2.one * _slime.Data.Scale,
                });

                onComplete?.Invoke();
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