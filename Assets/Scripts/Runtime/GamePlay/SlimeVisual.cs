using System;
using DG.Tweening;
using UnityEngine;

class SlimeVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slime _slime;
    [Header("Squash & Stretch")]
    [SerializeField] private float _squashWidth = 1.28f;
    [SerializeField] private float _squashHeight = 0.72f;
    [SerializeField] private float _stretchWidth = 0.88f;
    [SerializeField] private float _stretchHeight = 1.18f;
    [SerializeField] private float _deformResponse = 20f;
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
    public Sequence PlayExplosion(float duration, Action oncomplete = null)
    {
        _seq?.Kill();
        _slime.Freeze();
        _slime.Collider.enabled = false;

        Vector3 originScale = new Vector3(
            _slime.Data.Scale,
            _slime.Data.Scale,
            1f);
        transform.localScale = originScale;

        PitController _pit = GetComponentInParent<PitController>();
        Vector3 pitCenter = _pit.Center;
        pitCenter.z = transform.position.z;

        float chargeDuration = Mathf.Max(0.12f, duration * 0.25f);
        float inflateDuration = Mathf.Max(0.2f, duration * 0.35f);

        _seq = DOTween.Sequence()
            .Append(transform.DOShakePosition(
                chargeDuration, 0.06f, 14, 55f, false, true))
            .Append(transform.DOMove(pitCenter, 0.18f).SetEase(Ease.InCubic))
            .AppendInterval(0.05f)
            .Append(transform.DOScale(originScale * 1.2f, inflateDuration * 0.4f)
                .SetEase(Ease.OutSine))
            .Append(transform.DOScale(
                new Vector3(originScale.x * 1.38f, originScale.y * 1.28f, 1f),
                inflateDuration * 0.25f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(originScale * 1.5f, inflateDuration * 0.35f)
                .SetEase(Ease.InSine))
            .Join(transform.DOShakePosition(
                inflateDuration * 0.35f, 0.035f, 12, 45f, false, true))
            .AppendCallback(() =>
            {
                _explosionVfxEvent.Play(new VFXContext
                {
                    Position = transform.position,
                    Scale = originScale * 1.5f,
                    OverridePosition = true,
                    OverrideScale = true
                });
            })
            .Append(transform.DOScale(originScale * 1.7f, 0.06f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(Vector3.zero, 0.08f).SetEase(Ease.InBack))
            .OnComplete(() => oncomplete?.Invoke());

        return _seq;
    }
    public Sequence PlayMergeEffect()
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

        return _seq;
    }

    [ContextMenu("TestPlayScoreCollectEffect")]
    public void TestPlayScoreCollectEffect()
    {
        PlayScoreCollectEffect(_slime.Destroy);
    }
    public Sequence PlayScoreCollectEffect(Action onComplete = null)
    {
        transform.DOKill();

        Vector3 originScale = transform.localScale;

        SpriteRenderer sr = _slime.Sr;

        _seq = DOTween.Sequence()
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

        return _seq;
    }

    public void UpdateSquashStretch(float velocityY, Vector3 originScale)
    {
        if (_seq != null && _seq.IsActive()) return;

        Vector3 targetScale = originScale;

        if (velocityY > 0.2f)
        {
            // Bay lên: bè ngang và thấp xuống.
            float strength = Mathf.Sqrt(
                Mathf.InverseLerp(0.2f, 5f, velocityY));
            targetScale.x *= Mathf.Lerp(1f, _squashWidth, strength);
            targetScale.y *= Mathf.Lerp(1f, _squashHeight, strength);
        }
        else if (velocityY < -0.2f)
        {
            // Rơi xuống: hẹp ngang và dài theo chiều dọc.
            float strength = Mathf.InverseLerp(0.5f, 12f, -velocityY);
            targetScale.x *= Mathf.Lerp(1f, _stretchWidth, strength);
            targetScale.y *= Mathf.Lerp(1f, _stretchHeight, strength);
        }

        float blend = 1f - Mathf.Exp(-_deformResponse * Time.deltaTime);
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            blend);
    }

    public Sequence PlayDestroyEffect()
    {
        _seq?.Kill(); 
        _seq = DOTween.Sequence();
        return _seq;
    }
    public Sequence PlaySpawnEffect(Action onComplete = null)
    {
        _seq?.Kill();
        Vector3 targetScale = Vector3.one * _slime.Data.Scale;
        targetScale.z = 1f;
        Vector3 startScale = new Vector3(
            targetScale.x * 0.7f,
            targetScale.y * 0.7f,
            1f);
        Vector3 overshootScale = new Vector3(
            targetScale.x * 1.12f,
            targetScale.y * 1.12f,
            1f);
        transform.localScale = startScale;

        _seq = DOTween.Sequence()
            .Append(transform.DOScale(overshootScale, 0.16f).SetEase(Ease.OutBack))
            .Append(transform.DOScale(targetScale, 0.09f).SetEase(Ease.OutQuad))
            .OnComplete(() => onComplete?.Invoke());
        return _seq;
    }
}
