using UnityEngine;

public class PitController : MonoBehaviour
{
    [SerializeField] private Transform _content;
    private CompositeCollider2D _compositeColl;
    public float _highestContentY;

    public float TopYpit => _compositeColl.bounds.max.y;
    public float HeighestContentY => _highestContentY;
    public bool HasOverflowed => _highestContentY > TopYpit;


    void Awake()
    {
        _compositeColl = GetComponent<CompositeCollider2D>();

    }

    void Update()
    {
        CheckHeighestContentY();
    }

    public void AddToPit(GameObject obj)
    {
        obj.transform.SetParent(_content,true);
    }

    public void ClearAllContent()
    {
        foreach(Transform t in _content)
        {
            t.GetComponent<IDestroyable>()?.Destroy();
        }
        _highestContentY = 0;
    }

    private void CheckHeighestContentY()
    {
        Vector3 topPitpos = transform.position;
        topPitpos.y = TopYpit;
        Vector2 size = _compositeColl.bounds.size;
        float dis = (TopYpit-_compositeColl.bounds.min.y);
        RaycastHit2D hit  = Physics2D.BoxCast(topPitpos,new Vector2(size.x,0.1f),0,Vector2.down,dis,LayerMask.GetMask("Slime"));
        if(hit.collider == null) return;
        Slime slime = hit.collider.GetComponent<Slime>();
        
        if(slime !=null && slime.IsTouching) 
            _highestContentY = hit.collider.bounds.max.y;


    }

    


    void OnDrawGizmosSelected()
    {
        if(_compositeColl == null) return;
        Gizmos.color = Color.green;
        Vector2 start = _compositeColl.bounds.min;
        Vector2 end = start;
        end.y = _highestContentY;
        Gizmos.DrawLine(start,end);

        Gizmos.color = Color.red;
        start = transform.position;
        start.y = TopYpit;
        Vector2 size = _compositeColl.bounds.size;
        end = start;
        float dis = (TopYpit-_compositeColl.bounds.min.y);
        end.y -= dis;
        Gizmos.DrawLine(start,end);
    }

}
