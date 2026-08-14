using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_Move_Vfx_Event", menuName = "Events/Target Move Vfx Event")]
public class TargetMoveVfxEvent : BaseVfxEvent
{
    [SerializeField]private string _targetNameElement;

    protected override void OnBeforePlay(ParticleSystem particleSystem, VFXContext ctx)
    {
        Transform target = HudManager.Instance.GetUiByNameElement(_targetNameElement)?.transform;
        if (target == null) return;
        DOTween.Kill(particleSystem.transform);
        DOTween.Sequence()
            .Append(particleSystem.transform.DOMove(target.position, ctx.Duration))
            .SetEase(Ease.Linear);
    }
}