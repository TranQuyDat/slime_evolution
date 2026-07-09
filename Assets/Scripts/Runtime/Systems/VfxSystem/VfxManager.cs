using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


class VfxManager : MonoBehaviour
{
    public static VfxManager Instance;
    private Dictionary<ParticleSystem,Queue<ParticleSystem>> _vfxPools;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _vfxPools = new Dictionary<ParticleSystem, Queue<ParticleSystem>>();
    }

    public ParticleSystem Get( ParticleSystem prefab)
    {
        if (!_vfxPools.TryGetValue(prefab, out var pool))
        {
            pool = new Queue<ParticleSystem>();
            _vfxPools.Add(prefab, pool);
        }

        if (pool.Count > 0)
        {
            var ps = pool.Dequeue();
            ps.gameObject.SetActive(true);
            return ps;
        }

        ParticleSystem instance = Instantiate(prefab, transform);

        return instance;
    }
    public void Release(ParticleSystem prefab , ParticleSystem obj)
    {
        StartCoroutine(ReleaseWhenFinish(prefab,obj));
    }

    private IEnumerator  ReleaseWhenFinish( ParticleSystem prefab , ParticleSystem obj)
    {
        while (obj!= null && obj.IsAlive(true))
        {
            yield return null;
        }
        obj.gameObject.SetActive(false);
        _vfxPools[prefab].Enqueue(obj);
        obj.transform.SetParent(transform,true);
    }


}