class GoalProgress
{
    private string _goalId;
    private GoalData.Type _goalType;
    private QuestionProgress[] _questionProgresses;

    public string GoalId => _goalId;
    public GoalData.Type GoalType => _goalType;
    public QuestionProgress[] QuestionProgresses => _questionProgresses;

    public void Init(GoalData goalData)
    {
        _goalId = goalData.GoalId;
        _goalType = goalData.GoalType;
        _questionProgresses = new QuestionProgress[goalData.Questions.Length];
        for (int i = 0; i < _questionProgresses.Length; i++)
        {
            _questionProgresses[i] = new QuestionProgress(goalData.Questions[i]);
        }
    }
}
