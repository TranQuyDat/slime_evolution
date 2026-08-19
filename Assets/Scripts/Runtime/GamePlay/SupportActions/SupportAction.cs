using System;

abstract class SupportAction
{
    public virtual void OnUpdate()
    {
    }
    public abstract void OnEnter();
    public abstract void OnAction(Action Oncomplete = null);
    public abstract void OnFinish();
}