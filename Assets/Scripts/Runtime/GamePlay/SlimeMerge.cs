using UnityEngine;

class SlimeMerge : MonoBehaviour
{
    private Slime _thisSlime;
    [SerializeField] private GameObject _prefabSlime;
    private int _nextLV;
    void Awake()
    {
        _thisSlime = GetComponent<Slime>();
    }
    void Start()
    {
        _nextLV = _thisSlime.Data.Lv +1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Slime otherSlime = collision.gameObject.GetComponent<Slime>();
        if(otherSlime != null && otherSlime.Data.Lv == _thisSlime.Data.Lv)
        {
            // merge
            mergeSlime(otherSlime);
        }
    }

    private void mergeSlime(Slime OtherSlime)
    {
        if(this.GetInstanceID() > OtherSlime.GetInstanceID()) return;
        GameObject newSlimeobj = Instantiate(_prefabSlime,transform.position,Quaternion.identity);
        Slime newSlime = newSlimeobj.GetComponent<Slime>();
        newSlime.Init(new SlimeData(_nextLV));
        newSlimeobj.transform.SetParent(transform.parent,true);
        //destroy
        Destroy(this.gameObject);
        Destroy(OtherSlime.gameObject);
    }
}