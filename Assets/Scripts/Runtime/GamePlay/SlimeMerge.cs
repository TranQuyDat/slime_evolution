using UnityEngine;

class SlimeMerge : MonoBehaviour
{
    private GamePlay _gamePlay;
    private Slime _slime;
    private GameObject _slimePrefab;
    private SpawnSystem _spawnSystem;
    private int _nextLV;
    private int _maxLv;
    public bool IsMerging {get;set;} = false;  
    public Slime Slime => _slime;

    private BaseAudioEvent _mergeAudioEvent;
    private BaseVfxEvent _mergeVfxEvent;
    void Awake()
    {
        _slime = GetComponent<Slime>();
        _spawnSystem = GameManager.Instance.GetComponent<SpawnSystem>();
        _slimePrefab = _spawnSystem.SlimeDatabase.SlimePrefab;
        _gamePlay = GameManager.Instance.GamePlay;

        _mergeAudioEvent= Resources.Load<BaseAudioEvent>("Events/Merge_Audio_Event");
        _mergeVfxEvent =Resources.Load<BaseVfxEvent>("Events/Merge_Vfx_Event");
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
        GameObject newSlimeobj = ObjectPoolSystem.Instance.Order(_slimePrefab,"Slime");
        newSlimeobj.transform.rotation = Quaternion.identity;
        newSlimeobj.transform.position = transform.position;
        Slime newSlime = newSlimeobj.GetComponent<Slime>();
       
        SlimeDatabase slimeDatabase = _spawnSystem.SlimeDatabase;
        SlimeData slimeData = slimeDatabase.SlimeDatas[_nextLV];
       
        newSlime.Init(slimeData);
        newSlimeobj.transform.SetParent(transform.parent,true);
        if(newSlime.Data.Lv == (int)SlimeDatabase.SlimeType.Dragon)
            GameEvents.OnDragonExploded.Invoke(newSlime);
        _gamePlay.CalScoreByLevel(newSlime.Data.Lv);

        //sound
        _mergeAudioEvent.Play();
        //Vfx
        Collider2D col = newSlimeobj.GetComponent<Collider2D>();
        
        _mergeVfxEvent.Play(new()
        {
           Position =  col.transform.TransformPoint(col.offset),
           Speed = 3f,
           Scale = Vector2.one* newSlimeobj.transform.localScale,
        });
        //destroy
        _slime.Destroy();
        Other.Slime.Destroy();
    }
}