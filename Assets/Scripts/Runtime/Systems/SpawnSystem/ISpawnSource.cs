
interface ISpawnSource<T>
{
    public T GetNext();
    public T PeekNext();
}