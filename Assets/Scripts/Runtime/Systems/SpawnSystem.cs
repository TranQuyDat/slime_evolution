using UnityEngine;

class SpawnSystem : MonoBehaviour
{
    [SerializeField] private GameObject _slimePrefab;
    private int[] _spawnPattern  = {1,1,2,1,2,3,2,3,4};
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
    }

    private SlimeData RandomSlimeData()
    {
        SlimeData data;
        int randomLv = (_spawnIndex + 1) % _spawnPattern.Length;
        data = new SlimeData(randomLv);
        return data;
    }
}