using System;
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
    bool IsSlimeHoverInRemoveList => _slimesToRemove.Contains(_hoveredSlime);
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
        if(!IsMouseInPit()) return;

        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        if(_inputSystem.TryRaycastMouse2D<Slime>(mousePos,out Slime slime,LayerMask.GetMask("Slime")))
        {
            if(_hoveredSlime != null && _hoveredSlime != slime 
            && !IsSlimeHoverInRemoveList)
            {
                UnHighlightSlime(_hoveredSlime);
            }
            _hoveredSlime = slime;
            HighlightSlime(slime);
        }
        else if(_hoveredSlime != null && !IsSlimeHoverInRemoveList)
        {
            UnHighlightSlime(_hoveredSlime);
            _hoveredSlime = null;
        }
        else if(slime == null && _hoveredSlime != null)
        {
            _hoveredSlime = null;
        }
    }

    public override void OnEnter()
    {
        _inputSystem.BindAction(KeyCode.Mouse0,HandleBindRemoveAddSlime);
        _gameManager.Hud.SendCommand(CommandType.UpdateRemoveSlimesText,0);
    }

    public override void OnAction(Action Oncomplete = null)
    {
        if(_slimesToRemove.Count == 0)
        {
            return;
        }
        foreach(Slime slime in _slimesToRemove)
        {
            UnHighlightSlime(slime);
            slime.Destroy();
        }
        Oncomplete?.Invoke();
        OnFinish();

    }
    public override void OnFinish()
    {
        foreach (Slime slime in _slimesToRemove)
            UnHighlightSlime(slime);

        if (_hoveredSlime != null)
            UnHighlightSlime(_hoveredSlime);

        _hoveredSlime = null;
        _slimesToRemove.Clear();
        _inputSystem.UnbindAction(KeyCode.Mouse0,HandleBindRemoveAddSlime);
        
    }

    private bool IsMouseInPit()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return _pitCtrl.Bounds.Contains(mousePos);
    }

    private void HandleBindRemoveAddSlime()
    {
        if(_hoveredSlime == null ) return;
        if (!IsSlimeHoverInRemoveList)
        {
            AddSlimeToRemovalList();
        }
        else
        {
            RemoveSlimeFromRemovalList();
        }
    }

    private void AddSlimeToRemovalList()
    {
        if(_slimesToRemove.Count >= MaxSlimes_To_Remove)
        {
            _hoveredSlime = null;
            return;
        }
        _slimesToRemove.Add(_hoveredSlime);
        _gameManager.Hud.SendCommand(CommandType.UpdateRemoveSlimesText,
        _slimesToRemove.Count);

    }
    private void RemoveSlimeFromRemovalList()
    {
        if(_hoveredSlime)
        _slimesToRemove.Remove(_hoveredSlime);
        _gameManager.Hud.SendCommand(CommandType.UpdateRemoveSlimesText,
        _slimesToRemove.Count);

    }
    private void HighlightSlime(Slime slime)
    {
        if(slime == null) return;
        slime.Material.SetFloat("_UseOutline", 1f);
    }
    private void UnHighlightSlime(Slime slime)
    {
        if(slime == null) return;
        slime.Material.SetFloat("_UseOutline", 0f);
    }
}
