using UnityEngine;

class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputSystem _inputSystem;
    public GamePlay _gamePlay;

    private SaveSystem _saveSystem;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _gamePlay = GetComponentInChildren<GamePlay>();
        _saveSystem = new SaveSystem();
        _saveSystem.Provider = new PlayerPrefsProvider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SaveHightScore(int score)
    {
        int hightScore = _saveSystem.Load<int>("hightscore");
        if(score <= hightScore) return;
        _saveSystem.Save<int>(score,"hightscore");
    }
}
