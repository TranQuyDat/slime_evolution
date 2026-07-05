using UnityEngine;

class SlimeMerge : MonoBehaviour
{
    private GamePlay _gamePlay;
    private Slime _slime;
    private Slime _slimePrefab;
    private SlimeSpawnManager _SlimeSpawn;
    private int _nextLV;
    private int _maxLv;
    public bool IsMerging {get;set;} = false;  
    public Slime Slime => _slime;
    void Awake()
    {
        _slime = GetComponent<Slime>();
        _SlimeSpawn = GameManager.Instance.GetComponent<SlimeSpawnManager>();
        _slimePrefab = _SlimeSpawn.SlimeDatabase.SlimePrefab;
        _gamePlay = GameManager.Instance.GamePlay;
    }
    void Start()
    {
        _maxLv = _SlimeSpawn.SlimeDatabase.SlimeDatas.Length;
    }
    void OnEnable()
    {
        IsMerging = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        _nextLV = _slime.Data.Lv +1;
        if(_nextLV > _maxLv || IsMerging) return;
        SlimeMerge other = collision.gameObject.GetComponent<SlimeMerge>();
        if(other != null && !other.IsMerging && 
           other.Slime.Data.Lv == _slime.Data.Lv)
        {
            IsMerging = true;
            other.IsMerging = true;
            // merge
            mergeSlime(other);
        }
    }

    private void mergeSlime(SlimeMerge Other)
    {
        if(this.GetInstanceID() > Other.GetInstanceID()) return;
        GameObject newSlimeobj = ObjectPoolSystem.Instance.Order(_slimePrefab.gameObject,_slimePrefab.PoolKey);
        newSlimeobj.transform.rotation = Quaternion.identity;
        newSlimeobj.transform.position = transform.position;
        Slime newSlime = newSlimeobj.GetComponent<Slime>();
       
        SlimeDatabase slimeDatabase = _SlimeSpawn.SlimeDatabase;
        SlimeData slimeData = slimeDatabase.SlimeDatas[_nextLV];
       
        newSlime.Init(slimeData);
        newSlimeobj.transform.SetParent(transform.parent,true);
        if(newSlime.Data.Lv == (int)SlimeDatabase.SlimeType.Dragon)
            GameEvents.OnDragonExploded.Invoke(newSlime);
        _gamePlay.CalScoreByLevel(newSlime.Data.Lv);

        //destroy
        _slime.Destroy();
        Other.Slime.Destroy();
    }
}