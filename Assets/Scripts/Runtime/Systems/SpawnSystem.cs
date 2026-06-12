using UnityEngine;

class SpawnSystem : MonoBehaviour
{
    [SerializeField] private GameObject _slimePrefab;
    private int[] _spawnPattern  = {1, 3, 2, 4, 1, 5, 3, 2, 5};
    private int _spawnIndex = 0;
    public bool _canSpawn = false;
    private Slime _slimeHolder;

    public Slime SlimeHolder => _slimeHolder;
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
        SlimeData data = RandomSlimeData();
        Vector3 spwnPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.9f,10));
        GameObject slimeObj = Instantiate(_slimePrefab, spwnPos, Quaternion.identity); // spawn
        Slime slime = slimeObj.GetComponent<Slime>();
        slime.Init(data); // init
        _slimeHolder = slime;
        _slimeHolder.Freeze();
    }

    private SlimeData RandomSlimeData()
    {
        SlimeData data;
        _spawnIndex = (_spawnIndex + 1) % _spawnPattern.Length;
        int randomLv = _spawnPattern[_spawnIndex];
        data = new SlimeData(randomLv);
        return data;
    }

    public void Reset()
    {
        _canSpawn = false;
        _spawnIndex = 0;
        if(_slimeHolder != null)
            Destroy(_slimeHolder.gameObject);
    }

    public void EmptyHolder() => _slimeHolder = null;
}