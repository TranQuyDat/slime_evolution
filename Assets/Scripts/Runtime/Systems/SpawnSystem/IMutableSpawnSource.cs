
using System.Collections.Generic;

interface IMutableSpawnSource<T> :ISpawnSource<T>
{
    public void SetItems(IEnumerable<T> values);
    public void AddItems(T item);
    public void RemoveItems(T item);
    public void Reset();
}