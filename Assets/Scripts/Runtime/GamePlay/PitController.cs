using DG.Tweening;
using UnityEngine;

class PitController : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [Header("Transition")]
    [SerializeField] private float _transitionStartY = -300f;
    [SerializeField] private float _transitionDuration = 0.65f;

    private CompositeCollider2D _compositeColl;
    public float _highestContentY;

    public float TopYpit => _compositeColl.bounds.max.y;
    public float HeighestContentY => _highestContentY;
    public bool HadOverflowed => _highestContentY > TopYpit;
    public Bounds Bounds => _compositeColl.bounds;
    public Vector3 Center => _compositeColl.bounds.center;

    void Awake()
    {
        _compositeColl = GetComponent<CompositeCollider2D>();
        _highestContentY = _compositeColl.bounds.min.y;
    }
    void Update()
    {
        CheckHeighestContentY();
    }    
    public void AddToPit(GameObject obj)
    {
        obj.transform.SetParent(_content,true);
    }

    public Sequence FxShowPit()
    {
        DOTween.Kill(transform);

        Vector3 targetPosition = transform.position;
        Vector3 startPosition = targetPosition;
        startPosition.y = _transitionStartY;
        transform.position = startPosition;

        return DOTween.Sequence()
            .Append(transform.DOMoveY(targetPosition.y, _transitionDuration)
                .SetEase(Ease.OutBack, 1.08f));
    }

    public void ClearAllContent()
    {
        foreach(Transform t in _content)
        {
            t.GetComponent<IDestroyable>()?.Destroy();
        }
        _highestContentY = _compositeColl.bounds.min.y;
    }

    private void CheckHeighestContentY()
    {
        Slime slime = GetSlimeAbove();
        if(slime == null && _highestContentY != _compositeColl.bounds.min.y)
        {   
            _highestContentY = _compositeColl.bounds.min.y;
            return;
        }
        if(slime == null) return;
        if(slime !=null && slime.IsTouching) 
            _highestContentY = slime.Collider.bounds.max.y;


    }
    public T[] GetAllContents<T>()
    {
        return _content.GetComponentsInChildren<T>();
    }

    public Slime GetSlimeAbove()
    {
        Vector3 topPitpos = transform.position;
        topPitpos.y = TopYpit;
        Vector2 size = _compositeColl.bounds.size;
        float dis = (TopYpit-_compositeColl.bounds.min.y);
        RaycastHit2D hit = Physics2D.BoxCast(
            topPitpos,
            new Vector2(size.x, 0.1f),
            0,
            Vector2.down,
            dis,
            LayerMask.GetMask("Slime")
        );

        return hit.collider?.GetComponentInParent<Slime>();
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
