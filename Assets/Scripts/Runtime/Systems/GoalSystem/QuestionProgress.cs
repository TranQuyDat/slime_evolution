using System;

class QuestionProgress
{
    private Question _question;
    private float _currentValue;

    public string Id => _question.Id;
    public float CurrentValue => _currentValue;
    public QuestionType Type => _question.Type;

    public QuestionProgress(Question question)
    {
        _question = question;
        _currentValue = 0;
    }

    public void UpdateProgress(float value, Action onComplete = null)
    {
        _currentValue = value;
        if (_currentValue >= _question.Target)
        {
            onComplete?.Invoke();
        }
    }
}
