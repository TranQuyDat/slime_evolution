using UnityEngine;

[CreateAssetMenu(fileName ="Symple_Vfx_Event" , menuName ="Events/Symple Vfx Event")]
class SympleVfxEvent : BaseVfxEvent
{
    [SerializeField]private ParticleSystem _prefab;

    public override void Play(VFXContext ctx = default)
    {
        var manager = VfxManager.Instance;
        ParticleSystem parSys = manager.Get(_prefab);
        parSys.transform.position = ctx.Position;
        parSys.transform.localScale = ctx.Scale;
        var main = parSys.main;
        main.simulationSpeed = ctx.Speed;
        parSys.transform.SetParent(ctx.Parent,true);

        parSys.Play();

        manager.Release(_prefab,parSys);

    }
}