using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

class GoalUI: MonoBehaviour
{
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _txtGoal;
    [SerializeField] private TextMeshProUGUI _txtProgress;

    private float _targetValue;

    private QuestionType _type;
    public QuestionType Type => _type;

    public void Init(QuestionType type, Sprite icon, string goalText, float targetValue)
    {
        _type = type;
        _iconImg.sprite = icon;
        _txtGoal.text = goalText;
        _targetValue = targetValue;
        _txtProgress.text = $"0/{targetValue:0}";
    }

    public void UpdateProgress(float progressValue)
    {
        _txtProgress.text = $"{progressValue:0}/{_targetValue:0}";

        if (progressValue >= _targetValue)
        {
            _txtProgress.text = $"{_targetValue:0}/{_targetValue:0} (Completed!)";
        }
    }
}