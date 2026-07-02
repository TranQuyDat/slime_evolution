using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class InputSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private Dictionary<KeyCode, Action> _binddings = new Dictionary<KeyCode, Action>();

    void Awake()
    {
        if(_camera == null) _camera = Camera.main;
    }
    void Update()
    {
        if(_binddings.Count == 0) return;
        foreach (var binding in _binddings)        
        {
            if (Input.GetKeyDown(binding.Key))            
            {
                binding.Value?.Invoke();
            }
        }
    }
    public void BindAction(KeyCode k, Action a)
    {
        if (_binddings.ContainsKey(k))
        {
            _binddings[k] += a;
        }
        else
        {
            _binddings.Add(k, a);
        }
    }

    public void UnbindAction(KeyCode k, Action a)
    {
        if (_binddings.ContainsKey(k))
        {
            _binddings[k] -= a;
            if (_binddings[k] == null)
            {
                _binddings.Remove(k);
            }
        }
    }

    public bool TryRaycastMouse2D<T>(Vector2 pos,out T obj,LayerMask mask) where T:Component
    {
        obj = null;
        Collider2D hit = Physics2D.OverlapPoint(pos,mask);
        if(hit != null)
        {
            obj  = hit.GetComponent<T>();
            
        }
        return obj != null;

    }

}