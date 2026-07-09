using System.Collections.Generic;
using UnityEngine;

class ObjectPoolSystem :MonoBehaviour
{
    public static ObjectPoolSystem Instance {get; private set;}
    private Dictionary<string,Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
         if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    ///<param name="key">
    /// 1. Key = "" => IPoolable.PoolKey
    /// 2. IPoolable.PoolKey = null => prefab.name  
    /// </param>
    public GameObject Order(GameObject prefab,string key)
    {
        GameObject newObj ;
        if (!_pools.TryGetValue(key,out var pool))
        {
            pool = new Queue<GameObject>();
            _pools.Add(key,pool);
            newObj = Instantiate(prefab);
        }
        else if(pool.Count > 0)
            newObj = pool.Dequeue();
        else newObj = Instantiate(prefab);

        newObj.name = prefab.name;
        newObj.SetActive(true);
        return newObj;

    }

    public void Cancel(GameObject obj,string key)
    {
        if (!_pools.TryGetValue(key,out var pool))
        {
            pool = new Queue<GameObject>();
            _pools.Add(key,pool);
        }
        if(pool.Contains(obj)) return;
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}