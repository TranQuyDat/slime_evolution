using System.Collections.Generic;
using UnityEngine;

class ObjectPoolSystem :MonoBehaviour
{
    public static ObjectPoolSystem Instance {get; private set;}
    private Dictionary<string,Queue<Component>> _pools = new Dictionary<string, Queue<Component>>();

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
    public T Order<T>(T prefab,string key) where T : Component
    {
        T newObj ;
        if (!_pools.TryGetValue(key,out var pool))
        {
            pool = new Queue<Component>();
            _pools.Add(key,pool);
            newObj = Instantiate(prefab);
        }
        else if(pool.Count > 0)
            newObj = (T)pool.Dequeue();
        else newObj = Instantiate(prefab);

        newObj.name = prefab.name;
        newObj.gameObject.SetActive(true);
        return newObj;

    }

    public void Cancel<T>(T obj,string key) where T : Component
    {
        if (!_pools.TryGetValue(key,out var pool))
        {
            pool = new Queue<Component>();
            _pools.Add(key,pool);
        }
        if(pool.Contains(obj)) return;
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}