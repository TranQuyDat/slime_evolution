using UnityEngine;
public class UIElement : MonoBehaviour
{    
    [SerializeField] private string _customName; 
    public string CustomName => _customName;
}