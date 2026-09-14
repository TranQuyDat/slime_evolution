using System.Collections.Generic;
using UnityEngine;

class ShuffleBagSpawnSource<T> : IMutableSpawnSource<T>
{
    private List<T> _items;
    private int _index = 0;
    public ShuffleBagSpawnSource(T[] values)
    {
        _items = new List<T>(values);
        StartNewCycle();
    }


    public T GetNext()
    {
        T item = _items[_index];

        _index++;

        if (_index >= _items.Count)
            StartNewCycle();

        return item;
    }

    public T PeekNext()
    {
        if(_index >= _items.Count) return default(T);
        return _items[_index];
    }

    public void AddItems(T item)
    {
        _items.Add(item);
    }

    public void RemoveItems(T item)
    {
        _items.Remove(item);
    }

    public void SetItems(IEnumerable<T> values)
    {
        _items.Clear();
        _items.AddRange(values);
        StartNewCycle();
    }

    private void StartNewCycle()
    {
        ShuffleBag();
        _index = 0;
    }
    private void ShuffleBag()
    {
        int n = _items.Count;
        while(n > 1)
        {
            n--;
            int i = Random.Range(0,n+1);
            var temp = _items[i];
            _items[i] = _items[n];
            _items[n] = temp;
        }
    }

    public void Reset()
    {
        StartNewCycle();
    }
}