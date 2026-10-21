using UnityEngine;

[CreateAssetMenu(fileName = "GoalData", menuName = "Data/GoalData")]
class GoalData : ScriptableObject
{
    public enum Type { Match, Achievement }
    public enum Difficulty { none, Easy, Medium, Hard }

    [SerializeField] private string _goalId;
    [SerializeField] private Type _type;
    [SerializeField] private Difficulty _difficulty;
    [SerializeField] private Question[] _questions;
    private int _currentQuestionsCount = -1;

    public string GoalId => _goalId;
    public Question[] Questions => _questions;
    public Type GoalType => _type;
    public Difficulty GoalDifficulty => _difficulty;


    void OnValidate()
    {
        if (_questions == null || _questions.Length == 0 ||
            _questions.Length == _currentQuestionsCount) return;

        for (int i = 0; i < _questions.Length; i++)
        {
            if (_questions[i] == null) continue;

            if (string.IsNullOrEmpty(_questions[i].Id) ||
                (i > 0 && _questions[i].Id == _questions[i - 1].Id))
            {
                _questions[i].GenerateId(i);
            }
        }

        _currentQuestionsCount = _questions.Length;
    }

    public void SetGoalId(string goalId)
    {
        _goalId = goalId;
    }

    [ContextMenu("Save")]
    private void Save() => EditorSaveData.Save($"{_goalId}", this );
    [ContextMenu("Load")]
    private void Load()=> EditorSaveData.Load($"{_goalId}", this);
    
}
