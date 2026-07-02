using System.Collections.Generic;
using UnityEngine;

class RemoveSlimeAction : SupportAction
{
    private PitController _pitCtrl;
    private InputSystem _inputSystem;
    private GameManager _gameManager;
    private List<Slime> _slimesToRemove;
    private Slime _hoveredSlime;
    private const int MaxSlimes_To_Remove = 3;
    private Camera _camera;
    public RemoveSlimeAction(PitController pitCtrl,InputSystem inputSystem)
    {
        _gameManager = GameManager.Instance;
        _pitCtrl = pitCtrl;
        _inputSystem = inputSystem;
        _slimesToRemove = new List<Slime>();
        _camera = Camera.main;
    }
    public override void OnUpdate()
    {
        if(!IsMouseInPit()
        || _slimesToRemove.Count >= MaxSlimes_To_Remove) return;

        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        if(_inputSystem.TryRaycastMouse2D<Slime>(mousePos,out Slime slime,LayerMask.GetMask("Slime")))
        {
            if(_hoveredSlime != null && _hoveredSlime != slime)
            {
                UnHighlightSlime(_hoveredSlime);
            }
            _hoveredSlime = slime;
            HighlightSlime(slime);
        }
        else _hoveredSlime = null;
    }

    public override void OnEnter()
    {
        _inputSystem.BindAction(KeyCode.Mouse0,AddSlimeToRemovalList);
        _gameManager.Hud.SendCommand(CommandType.UpdateRemoveSlimesText,0);
    }

    public override void OnAction()
    {
        if(_slimesToRemove.Count == 0)
        { 
            OnFinish();
            return;
        }
        foreach(var slime in _slimesToRemove)
        {
            slime.Destroy();
        }
        OnFinish();
    }
    public override void OnFinish()
    {
        _slimesToRemove.Clear();
        _inputSystem.UnbindAction(KeyCode.Mouse0,AddSlimeToRemovalList);
        _hoveredSlime = null;
    }

    private bool IsMouseInPit()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return _pitCtrl.Bounds.Contains(mousePos);
    }

    private void AddSlimeToRemovalList()
    {
        if(_hoveredSlime == null) return;
        if(_slimesToRemove.Count >= MaxSlimes_To_Remove)
        {
            _hoveredSlime = null;
            return;
        }
        if(_slimesToRemove.Contains(_hoveredSlime)) return;
        _slimesToRemove.Add(_hoveredSlime);
        _gameManager.Hud.SendCommand(CommandType.UpdateRemoveSlimesText,
        _slimesToRemove.Count);

    }
    private void HighlightSlime(Slime slime)
    {
        if(slime == null) return;
    }
    private void UnHighlightSlime(Slime slime)
    {
        if(slime == null) return;
    }
}