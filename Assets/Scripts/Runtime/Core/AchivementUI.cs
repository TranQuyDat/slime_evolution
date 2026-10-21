using UnityEngine;
using TMPro;
using UnityEngine.UI;

class AchivementUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _txtTitle;
    [SerializeField]private TextMeshProUGUI _txtProgress;
    [SerializeField]private TextMeshProUGUI _txtStatus;
    [SerializeField]private Image _imgIcon;

    private QuestionType _type;
    private float _targetValue;
    public QuestionType Type => _type;

    public void Init(QuestionType type, string title, float progressValue,float targetValue, Sprite icon )
    {
        _type = type;
        _txtTitle.text = title;
        _imgIcon.sprite = icon;

        _targetValue = targetValue;
        _txtProgress.text = $"{Mathf.Min(progressValue,targetValue)}/{targetValue}";
        
        _txtStatus.text = "Progressing";
        if(progressValue >= targetValue)
        {
            _txtStatus.text = "Completed";
        }
    }

    public void UpdateProgress(float value)
    {
        if(value > _targetValue) return;
        _txtProgress.text = $"{value}/{_targetValue}";
        if(value == _targetValue)
        {
            _txtStatus.text = "Completed";
        }
    }
}