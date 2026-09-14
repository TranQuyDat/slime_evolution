using System;
using System.Collections.Generic;
using UnityEngine;

class RemoveSlimeAction : SupportAction
{
    private const int MaxSlimesToRemove = 3;

    private readonly InputSystem _inputSystem;
    private readonly GameManager _gameManager;
    private readonly List<Slime> _selectedSlimes = new List<Slime>();
    private readonly Camera _camera;
    private readonly int _slimeLayerMask;
    private Slime _hoveredSlime;

    public RemoveSlimeAction(InputSystem inputSystem)
    {
        _gameManager = GameManager.Instance;
        _inputSystem = inputSystem;
        _camera = Camera.main;
        _slimeLayerMask = LayerMask.GetMask("Slime");
    }

    public override void OnUpdate()
    {
        if (_inputSystem.IsUsingTouch) return;

        Slime currentHover = GetSlimeUnderPointer();
        if (currentHover == _hoveredSlime) return;

        if (_hoveredSlime != null && !_selectedSlimes.Contains(_hoveredSlime))
            SetHighlight(_hoveredSlime, false);

        _hoveredSlime = currentHover;
        SetHighlight(_hoveredSlime, true);
    }

    public override void OnEnter()
    {
        SetHighlight(_hoveredSlime, false);
        _hoveredSlime = null;
        ClearSelection();
        UpdateSelectionCount();
    }

    public void HandleInput()
    {
        if (_inputSystem.IsPointerOverUI() ||
            !_inputSystem.TryGetPointerPosition(out Vector2 pointerPosition))
        {
            return;
        }

        Slime slime = GetSlimeUnderPointer(pointerPosition);
        if (slime == null) return;

        ToggleSelection(slime);
    }

    public override void OnAction(Action onComplete = null)
    {
        if (_selectedSlimes.Count == 0) return;

        for (int i = 0; i < _selectedSlimes.Count; i++)
        {
            Slime slime = _selectedSlimes[i];
            SetHighlight(slime, false);
            slime.Destroy();
        }

        _selectedSlimes.Clear();
        SetHighlight(_hoveredSlime, false);
        _hoveredSlime = null;
        UpdateSelectionCount();
        onComplete?.Invoke();
    }

    public override void OnFinish()
    {
        ClearSelection();
        SetHighlight(_hoveredSlime, false);
        _hoveredSlime = null;
        UpdateSelectionCount();
    }

    private void ToggleSelection(Slime slime)
    {
        int index = _selectedSlimes.IndexOf(slime);
        if (index >= 0)
        {
            _selectedSlimes.RemoveAt(index);
            bool keepHover = !_inputSystem.IsUsingTouch &&
                             slime == _hoveredSlime;
            SetHighlight(slime, keepHover);
        }
        else if (_selectedSlimes.Count < MaxSlimesToRemove)
        {
            _selectedSlimes.Add(slime);
            SetHighlight(slime, true);
        }

        UpdateSelectionCount();
    }

    private void ClearSelection()
    {
        for (int i = 0; i < _selectedSlimes.Count; i++)
            SetHighlight(_selectedSlimes[i], false);

        _selectedSlimes.Clear();
    }

    private void UpdateSelectionCount()
    {
        _gameManager.Hud.SendCommand(
            CommandType.UpdateRemoveSlimesText,
            _selectedSlimes.Count);
    }

    private Slime GetSlimeUnderPointer()
    {
        return _inputSystem.TryGetPointerPosition(out Vector2 pointerPosition)
            ? GetSlimeUnderPointer(pointerPosition)
            : null;
    }

    private Slime GetSlimeUnderPointer(Vector2 pointerPosition)
    {
        Vector3 worldPosition = _camera.ScreenToWorldPoint(pointerPosition);
        worldPosition.z = 0f;

        _inputSystem.TryRaycast2D(
            worldPosition,
            out Slime slime,
            _slimeLayerMask);
        return slime;
    }

    private static void SetHighlight(Slime slime, bool enabled)
    {
        if (slime == null) return;
        slime.Material.SetFloat("_UseOutline", enabled ? 1f : 0f);
    }
}
