using System.Collections.Generic;
using UnityEngine;

class SpawnSystem : MonoBehaviour
{
    [SerializeField] private GameObject _slimePrefab;
    private int[] _spawnPattern  = {1, 3, 2, 4, 1, 5, 3, 2, 5};
    [SerializeField] private SlimeDatabase _slimeDatabase; 
    private  List<SlimeDatabase.SlimeType> _spawnBag = new List<SlimeDatabase.SlimeType>();
    public bool _canSpawn = false;
    private Slime _slimeHolder;

    public Slime SlimeHolder => _slimeHolder;
    public SlimeDatabase SlimeDatabase => _slimeDatabase;
    private void Start()
    {
        
    }

    void Update()
    {
        if(_canSpawn)
        {
            SpawnSlime();
            _canSpawn = false;
        }
    }

    private void SpawnSlime()
    {
        int i = (int)NextSlime();
        SlimeData data = _slimeDatabase.SlimeDatas[i];

        Vector3 spwnPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.9f,10)); // pos mouse
        GameObject slimeObj = Instantiate(_slimePrefab, spwnPos, Quaternion.identity); // spawn

        Slime slime = slimeObj.GetComponent<Slime>();
        slime.Init(data); // init
        _slimeHolder = slime;
        _slimeHolder.Freeze();
    }

    private SlimeDatabase.SlimeType NextSlime()
    {
        if(_spawnBag.Count <=0) RefillBag();
        int r = Random.Range(0,_spawnBag.Count); 
        SlimeDatabase.SlimeType t = _spawnBag[r];
        _spawnBag.RemoveAt(r);
        return t;
    }

    private void RefillBag()
    {
        int[] ids = {0,0,0,0,0,0,0,0,
                    1,1,1,1,1,1,
                    2,2,2,2,
                    3,3};

        foreach(int i in ids)
        {
            _spawnBag.Add((SlimeDatabase.SlimeType)i);
        }

        ShuffleBag();
    }

    private void ShuffleBag()
    {
        int n = _spawnBag.Count;
        while(n > 1)
        {
            n--;
            int i = Random.Range(0,n+1);
            var temp = _spawnBag[i];
            _spawnBag[i] = _spawnBag[n];
            _spawnBag[n] = temp;
        }
    }

    public void Reset()
    {
        _canSpawn = false;
        _spawnBag.Clear();
        if(_slimeHolder != null)
            Destroy(_slimeHolder.gameObject);
    }

    public void EmptyHolder() => _slimeHolder = null;
}