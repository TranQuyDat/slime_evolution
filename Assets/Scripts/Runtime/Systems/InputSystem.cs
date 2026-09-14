using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
class InputEventBinding
{
    [SerializeField] private InputAction _input;
    [SerializeField] private UnityEvent _onInput;
    [SerializeField] private bool _ignoreWhenOverUI = true;

    private Func<bool> _isPointerOverUI;
    private Action<InputDevice> _onDeviceChanged;
    private bool _startedOverUI;

    public void Enable(
        Func<bool> isPointerOverUI,
        Action<InputDevice> onDeviceChanged)
    {
        _isPointerOverUI = isPointerOverUI;
        _onDeviceChanged = onDeviceChanged;
        _input.started += HandleStarted;
        _input.performed += HandlePerformed;
        _input.canceled += HandleCanceled;
        _input.Enable();
    }

    public void Disable()
    {
        _input.started -= HandleStarted;
        _input.performed -= HandlePerformed;
        _input.canceled -= HandleCanceled;
        _input.Disable();
        _isPointerOverUI = null;
        _onDeviceChanged = null;
        _startedOverUI = false;
    }

    private void HandleStarted(InputAction.CallbackContext context)
    {
        _onDeviceChanged?.Invoke(context.control.device);
        _startedOverUI = _ignoreWhenOverUI &&
                         _isPointerOverUI != null &&
                         _isPointerOverUI();
    }

    private void HandlePerformed(InputAction.CallbackContext context)
    {
        _onDeviceChanged?.Invoke(context.control.device);
        if (!_startedOverUI)
            _onInput?.Invoke();
    }

    private void HandleCanceled(InputAction.CallbackContext context)
    {
        _startedOverUI = false;
    }
}

class InputSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private InputEventBinding[] _bindings;

    private readonly List<RaycastResult> _uiHits = new List<RaycastResult>();
    private PointerEventData _pointerEventData;
    private Vector2 _lastPointerPosition;
    private bool _lastInputWasTouch;
    public bool IsUsingTouch => _lastInputWasTouch ||
                                SettingBeforeInGame.IsHandheld;
    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;
        _lastPointerPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    private void OnEnable()
    {
        if (_bindings == null) return;

        foreach (InputEventBinding binding in _bindings)
            binding?.Enable(IsPointerOverUI, SetInputDevice);
    }

    private void OnDisable()
    {
        if (_bindings == null) return;

        foreach (InputEventBinding binding in _bindings)
            binding?.Disable();
    }

    public bool TryGetPointerPosition(out Vector2 screenPosition)
    {
        if (Pointer.current != null)
            _lastPointerPosition = Pointer.current.position.ReadValue();

        screenPosition = _lastPointerPosition;
        return true;
    }

    private void SetInputDevice(InputDevice device)
    {
        _lastInputWasTouch = device is Touchscreen;
    }

    public bool IsPointerOverUI()
    {
        TryGetPointerPosition(out Vector2 position);
        if (EventSystem.current == null) return false;

        _uiHits.Clear();
        _pointerEventData ??= new PointerEventData(EventSystem.current);
        _pointerEventData.position = position;
        EventSystem.current.RaycastAll(_pointerEventData, _uiHits);

        for (int i = 0; i < _uiHits.Count; i++)
        {
            if (_uiHits[i].gameObject.GetComponentInParent<Button>() != null)
                return true;
        }

        return false;
    }

    public bool TryRaycast2D<T>(Vector2 position, out T target, LayerMask mask)
        where T : Component
    {
        target = null;
        Collider2D hit = Physics2D.OverlapPoint(position, mask);
        if (hit == null) return false;

        target = hit.GetComponent<T>();
        return target != null;
    }
}
