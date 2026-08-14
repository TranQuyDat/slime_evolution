using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    public static VfxManager Instance;

    private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> _pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public ParticleSystem Get(ParticleSystem prefab)
    {
        if (!_pools.TryGetValue(prefab, out var pool))
        {
            pool = new Queue<ParticleSystem>();
            _pools[prefab] = pool;
        }

        if (pool.Count > 0)
        {
            var instance = pool.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }

        return Instantiate(prefab, transform);
    }

    public void Release(ParticleSystem prefab, ParticleSystem instance)
    {
        if (instance == null) return;
        StartCoroutine(ReturnToPool(prefab, instance));
    }

    private IEnumerator ReturnToPool(ParticleSystem prefab, ParticleSystem instance)
    {
        while (instance != null && instance.IsAlive(true))
        {
            yield return null;
        }

        if (instance == null) yield break;

        instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform, true);
        _pools[prefab].Enqueue(instance);
    }
}