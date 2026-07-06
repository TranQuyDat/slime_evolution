using System.Collections.Generic;
using UnityEngine;

class SlimeSpawnManager : MonoBehaviour
{
    [SerializeField] private SlimeDatabase _slimeDatabase; 
    private ObjectPoolSystem _objectPoolSys;
    private IMutableSpawnSource<int> _bag;
    public SlimeDatabase SlimeDatabase => _slimeDatabase;
    private Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
        _objectPoolSys = ObjectPoolSystem.Instance;
        int[] i ={
            1,1,1,1,
            1,1,1,1,
            2,2,2,2,
            3,3  
        };
        _bag = new ShuffleBagSpawnSource<int>(i);
    }
    
    public Slime Spawn()
    {
        Slime slimePrefab =  _slimeDatabase.SlimePrefab;
        GameObject obj = _objectPoolSys.Order(slimePrefab.gameObject,slimePrefab.PoolKey); 
        obj.transform.position = _camera.ViewportToWorldPoint(new Vector2(0.5f,0.8f));
        obj.transform.rotation = Quaternion.identity;
       
        Slime newSlime = obj.GetComponent<Slime>();
        int id = _bag.GetNext();
        SlimeData data = _slimeDatabase.SlimeDatas[id];
        newSlime.Init(data);
        newSlime.Freeze();
        return newSlime;
    }
    public SlimeData PreviewNextSlime()
    {
        int id = _bag.PeekNext();
        SlimeData data = _slimeDatabase.SlimeDatas[id];
        return data;
    }
    
    public void Reset()
    {
        int[] i = {
            1,1,1,1,
            1,1,1,1,
            2,2,2,2,
            3,3  
        }; 
        _bag.SetItems(i);
        _bag.Reset();
    }

}