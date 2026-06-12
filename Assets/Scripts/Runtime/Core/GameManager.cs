using UnityEngine;

class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputSystem _inputSystem;
    public GamePlay _gamePlay;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _gamePlay = GetComponentInChildren<GamePlay>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
