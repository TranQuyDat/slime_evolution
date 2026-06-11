using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class InputSystem : MonoBehaviour
{
    private Dictionary<KeyCode, Action> _binddings = new Dictionary<KeyCode, Action>();
    
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

}