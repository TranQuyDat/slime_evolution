using UnityEngine;

class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputSystem _inputSystem;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
