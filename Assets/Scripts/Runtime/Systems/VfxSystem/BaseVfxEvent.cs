using UnityEngine;

public abstract class BaseVfxEvent : ScriptableObject
{
    [SerializeField] private ParticleSystem _prefab;
    [SerializeField] private VFXContext _defaultContext = new()
    {
        Scale = Vector3.one,
        Speed = 1f,
        OverrideScale = true,
        OverrideSpeed = true
    };

    public void Play(VFXContext ctx = default)
    {
        var mergedCtx = _defaultContext.Merge(ctx);
        var manager = VfxManager.Instance;

        if (manager == null)
        {
            Debug.LogError("VfxManager not found.");
            return;
        }

        var particleSystem = manager.Get(_prefab);
        ApplyToParticleSystem(particleSystem, mergedCtx);
        OnBeforePlay(particleSystem, mergedCtx);

        particleSystem.Play();
        manager.Release(_prefab, particleSystem);
    }

    protected virtual void ApplyToParticleSystem(ParticleSystem particleSystem, VFXContext ctx)
    {
        var t = particleSystem.transform;
        t.position = ctx.Position;
        t.localScale = ctx.Scale;

        if (_defaultContext.OverrideParent)
        {
            t.SetParent(ctx.Parent, true);
        }
        else
        {
            t.SetParent(null, true);
        }

        var main = particleSystem.main;

        if (_defaultContext.OverrideSpeed)
        {
            main.simulationSpeed = ctx.Speed;
        }

        if (_defaultContext.OverrideDuration)
        {
            main.duration = ctx.Duration;
        }

        if (_defaultContext.OverrideColor)
        {
            main.startColor = ctx.Color;
        }
    }

    protected virtual void OnBeforePlay(ParticleSystem particleSystem, VFXContext ctx)
    {
    }
}