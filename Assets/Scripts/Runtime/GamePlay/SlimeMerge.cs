using UnityEngine;

class SlimeMerge : MonoBehaviour
{
    private Slime _thisSlime;
    [SerializeField] private GameObject _prefabSlime;
    private SpawnSystem _spawnSystem;
    private int _nextLV;
    private int _maxLv;
    private bool _isMerging;
    void Awake()
    {
        _thisSlime = GetComponent<Slime>();
        _spawnSystem = GameManager.Instance.GetComponent<SpawnSystem>();
    }
    void Start()
    {
        _maxLv = _spawnSystem.SlimeDatabase.SlimeDatas.Length;
        _nextLV = _thisSlime.Data.Lv +1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if(_nextLV > _maxLv) return;
        Slime otherSlime = collision.gameObject.GetComponent<Slime>();
        if(otherSlime != null && otherSlime.Data.Lv == _thisSlime.Data.Lv)
        {
            // merge
            mergeSlime(otherSlime);
        }
    }

    private void mergeSlime(Slime OtherSlime)
    {
        if(_isMerging) return;
        if(this.GetInstanceID() > OtherSlime.GetInstanceID()) return;
        _isMerging = true;
        GameObject newSlimeobj = Instantiate(_prefabSlime,transform.position,Quaternion.identity);
        Slime newSlime = newSlimeobj.GetComponent<Slime>();
       
        SlimeDatabase slimeDatabase = _spawnSystem.SlimeDatabase;
        SlimeData slimeData = slimeDatabase.SlimeDatas[_nextLV];
       
        newSlime.Init(slimeData);
        newSlimeobj.transform.SetParent(transform.parent,true);
       
        //destroy
        Destroy(this.gameObject);
        Destroy(OtherSlime.gameObject);
    }
}