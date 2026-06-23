using UnityEngine;

class SlimeMerge : MonoBehaviour
{
    private Slime _slime;
    private GameObject _slimePrefab;
    private SpawnSystem _spawnSystem;
    private int _nextLV;
    private int _maxLv;
    public bool IsMerging {get;set;} = false;  
    public Slime Slime => _slime;
    void Awake()
    {
        _slime = GetComponent<Slime>();
        _spawnSystem = GameManager.Instance.GetComponent<SpawnSystem>();
        _slimePrefab = _spawnSystem.SlimeDatabase.SlimePrefab;
    }
    void Start()
    {
        _maxLv = _spawnSystem.SlimeDatabase.SlimeDatas.Length;
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
        GameObject newSlimeobj = ObjectPoolSystem.Instance.Order(_slimePrefab);
        newSlimeobj.transform.rotation = Quaternion.identity;
        newSlimeobj.transform.position = transform.position;
        Slime newSlime = newSlimeobj.GetComponent<Slime>();
       
        SlimeDatabase slimeDatabase = _spawnSystem.SlimeDatabase;
        SlimeData slimeData = slimeDatabase.SlimeDatas[_nextLV];
       
        newSlime.Init(slimeData);
        newSlimeobj.transform.SetParent(transform.parent,true);
        if(newSlime.Data.Lv == (int)SlimeDatabase.SlimeType.Dragon)
            GameEvents.OnDragonExploded.Invoke(newSlime);
        //destroy
        _slime.Destroy();
        Other.Slime.Destroy();
    }
}