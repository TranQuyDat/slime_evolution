using UnityEngine;

class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]private InputSystem _inputSystem;
    [SerializeField]private GamePlay _gamePlay;

    public InputSystem InputSystem => _inputSystem;
    public GamePlay GamePlay => _gamePlay;
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
