using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

class PitController : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [Header("Out Of Bounds")]
    [SerializeField] private float _despawnOffsetBelowBottom = 2f;
    [Header("Transition")]
    [SerializeField] private float _transitionStartY = -300f;
    [SerializeField] private float _transitionDuration = 0.65f;

    private CompositeCollider2D _compositeColl;
    private readonly List<Slime> _slimeBuffer = new();
    public float _highestContentY;

    public float TopYpit => _compositeColl.bounds.max.y;
    public float HeighestContentY => _highestContentY;
    public bool HadOverflowed => _highestContentY > TopYpit;
    public Bounds Bounds => _compositeColl.bounds;
    public Vector3 Center => _compositeColl.bounds.center;
    public float DespawnY =>
        _compositeColl.bounds.min.y - _despawnOffsetBelowBottom;

    void Awake()
    {
        _compositeColl = GetComponent<CompositeCollider2D>();
        _highestContentY = _compositeColl.bounds.min.y;
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

    public T[] GetAllContents<T>()
    {
        return _content.GetComponentsInChildren<T>();
    }

    public Slime GetSlimeAbove()
    {
        Slime highestSlime = null;
        float highestY = float.NegativeInfinity;

        _slimeBuffer.Clear();
        _content.GetComponentsInChildren(false, _slimeBuffer);

        foreach (Slime slime in _slimeBuffer)
        {
            if (slime == null || slime.IsDestroying || !slime.Collider.enabled ||
                !slime.IsTouching)
                continue;

            float slimeTopY = slime.Collider.bounds.max.y;
            if (slimeTopY <= highestY) continue;

            highestY = slimeTopY;
            highestSlime = slime;
        }

        _highestContentY = highestSlime == null
            ? _compositeColl.bounds.min.y
            : highestY;
        return highestSlime;
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

        Gizmos.color = Color.yellow;
        float halfWidth = _compositeColl.bounds.extents.x;
        Vector3 despawnLeft = new Vector3(
            _compositeColl.bounds.center.x - halfWidth, DespawnY, 0f);
        Vector3 despawnRight = new Vector3(
            _compositeColl.bounds.center.x + halfWidth, DespawnY, 0f);
        Gizmos.DrawLine(despawnLeft, despawnRight);
    }

}
