using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

abstract class UItransitionBase
{
    public virtual Sequence FxClickBtn(Transform btn, float duration = 0.15f)
    {
        Vector3 oriScale = btn.localScale;
        DOTween.Kill(btn);

        return DOTween.Sequence()
            .Append(btn.DOScale(oriScale * 0.9f, duration * 0.35f).SetEase(Ease.OutQuad))
            .Append(btn.DOScale(oriScale, duration * 0.65f).SetEase(Ease.OutBack));
    }

    public Sequence FxClickBtn(Transform btn, Action onComplete, float duration = 0.15f)
    {
        return FxClickBtn(btn, duration).OnComplete(() => onComplete?.Invoke());
    }

    public Sequence FxShowButtonPop(Transform button, float duration = 0.2f)
    {
        Vector3 targetScale = button.localScale;
        return FxPop(button, targetScale, 1.15f, duration);
    }

    protected Tween FxFadeOverlay(
        Transform panel,
        float targetAlpha,
        float duration)
    {
        Image overlay = GetOrCreateOverlay(panel);
        return overlay.DOFade(targetAlpha, duration);
    }

    protected Sequence FxSlideFade(
        Transform target,
        Vector3 offset,
        float duration,
        Ease ease = Ease.OutCubic)
    {
        DOTween.Kill(target);
        CanvasGroup group = GetOrAddCanvasGroup(target.gameObject);
        Vector3 targetPosition = target.localPosition;
        target.localPosition = targetPosition + offset;
        group.alpha = 0f;

        return DOTween.Sequence()
            .Join(target.DOLocalMove(targetPosition, duration).SetEase(ease))
            .Join(group.DOFade(1f, duration));
    }

    protected Tween FxScale(
        Transform target,
        float startScale,
        float duration,
        Ease ease = Ease.OutBack)
    {
        DOTween.Kill(target);
        Vector3 targetScale = target.localScale;
        target.localScale = targetScale * startScale;
        return target.DOScale(targetScale, duration).SetEase(ease);
    }

    protected Tween FxMoveFrom(
        Transform target,
        Vector3 offset,
        float duration,
        Ease ease = Ease.OutCubic)
    {
        DOTween.Kill(target);
        Vector3 targetPosition = target.localPosition;
        target.localPosition += offset;
        return target.DOLocalMove(targetPosition, duration).SetEase(ease);
    }

    protected Sequence FxPop(
        Transform target,
        Vector3 targetScale,
        float overshoot = 1.15f,
        float duration = 0.5f)
    {
        DOTween.Kill(target);
        target.localScale = Vector3.zero;
        return DOTween.Sequence()
            .Append(target.DOScale(targetScale * overshoot, duration * 0.7f)
                .SetEase(Ease.OutBack))
            .Append(target.DOScale(targetScale, duration * 0.3f)
                .SetEase(Ease.OutQuad));
    }

    protected Sequence FxScaleFade(
        Transform target,
        float startScale,
        float duration,
        Ease ease = Ease.OutBack)
    {
        CanvasGroup group = GetOrAddCanvasGroup(target.gameObject);
        group.alpha = 0f;
        return DOTween.Sequence()
            .Join(FxScale(target, startScale, duration, ease))
            .Join(group.DOFade(1f, duration));
    }

    protected Sequence FxSlideScale(
        Transform target,
        Vector3 offset,
        float startScale,
        float duration,
        Ease moveEase = Ease.OutCubic,
        Ease scaleEase = Ease.OutQuad)
    {
        DOTween.Kill(target);
        Vector3 targetPosition = target.localPosition;
        Vector3 targetScale = target.localScale;
        target.localPosition = targetPosition + offset;
        target.localScale = targetScale * startScale;

        return DOTween.Sequence()
            .Join(target.DOLocalMove(targetPosition, duration).SetEase(moveEase))
            .Join(target.DOScale(targetScale, duration).SetEase(scaleEase));
    }

    protected static CanvasGroup GetOrAddCanvasGroup(GameObject target)
    {
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        return group != null ? group : target.AddComponent<CanvasGroup>();
    }

    private static Image GetOrCreateOverlay(Transform panel)
    {
        const string overlayName = "TransitionOverlay";
        Transform existing = panel.Find(overlayName);
        Image overlay;

        if (existing == null)
        {
            GameObject overlayObject = new GameObject(
                overlayName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            overlayObject.transform.SetParent(panel, false);
            overlayObject.transform.SetAsFirstSibling();

            RectTransform rect = overlayObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            overlay = overlayObject.GetComponent<Image>();
        }
        else
        {
            overlay = existing.GetComponent<Image>();
        }

        DOTween.Kill(overlay);
        overlay.color = new Color(0f, 0f, 0f, 0f);
        overlay.raycastTarget = true;
        return overlay;
    }
}
