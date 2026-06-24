using UnityEngine;
abstract class IState : MonoBehaviour
{
    protected GameManager _gameManager;
    protected HudManager _hud;
    public abstract void Enter();
    public abstract void Exit();
}
