using System.Threading.Tasks;
using Unity.Mathematics;
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

    private BaseAudioEvent _mergeAudioEvent;
    private Delay _delay;
    void Awake()
    {
        _slime = GetComponent<Slime>();
        _SlimeSpawn = GameManager.Instance.GetComponent<SlimeSpawnManager>();
        _slimePrefab = _SlimeSpawn.SlimeDatabase.SlimePrefab;
        _gamePlay = GameManager.Instance.GamePlay;
        _delay = new();
        _mergeAudioEvent= Resources.Load<BaseAudioEvent>("Events/Merge_Audio_Event");
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
        if(_slime.IsDestroying || _gamePlay.IsGameOver) return;
        _nextLV = _slime.Data.Lv +1;
        if(_nextLV >= _maxLv || IsMerging) return;
        SlimeMerge other = collision.gameObject.GetComponent<SlimeMerge>();
        if(other == null || other.IsMerging ||
           other.Slime.Data.Lv != _slime.Data.Lv) return;

        // Chỉ object có instance ID nhỏ hơn chịu trách nhiệm merge.
        // Phải kiểm tra trước khi đánh dấu IsMerging để tránh cả hai cùng bị khóa.
        if(GetInstanceID() > other.GetInstanceID()) return;

        IsMerging = true;
        other.IsMerging = true;
        mergeSlime(other);
    }

    private async void mergeSlime(SlimeMerge Other)
    {
        Vector2 pos = (transform.position + Other.transform.position)/2f;
        Slime newSlime = ObjectPoolSystem.Instance.Order<Slime>(_slimePrefab,
        _slimePrefab.PoolKey);
        newSlime.transform.rotation = Quaternion.identity;
        newSlime.transform.position = pos;
       
        SlimeDatabase slimeDatabase = _SlimeSpawn.SlimeDatabase;
        SlimeData slimeData = slimeDatabase.SlimeDatas[_nextLV];
       
        newSlime.Init(slimeData);
        newSlime.Unfreeze();
        newSlime.transform.SetParent(transform.parent,true);
        //sound
        _mergeAudioEvent.Play();
        //Vfx
        newSlime.Visual.PlayMergeEffect();
        //destroy
        _slime.Destroy();
        Other.Slime.Destroy();

        if(newSlime.Data.Lv == (int)SlimeDatabase.SlimeType.Dragon) 
            GameEvents.OnDragonExploded.Invoke(newSlime,
            (int s)=>_gamePlay.CalScoreByLevel(s,pos));
        _gamePlay.CalScoreByLevel(newSlime.Data.Lv,pos);
        _gamePlay.OnSlimeMerged(newSlime.Data.Lv);

    }
}
