abstract class SupportAction
{
    public virtual void OnUpdate()
    {
    }
    public abstract void OnEnter();
    public abstract void OnAction();
    public abstract void OnFinish();
}