using System;
using UnityEngine;
[Serializable]
class Question
{
    [SerializeField] private string _id;
    [SerializeField] private QuestionType _type;
    [TextArea][SerializeField] private string _question;
    [SerializeField] private float _target = 1;

    public string Id => _id;
    public QuestionType Type => _type;
    public float Target => _target;
    public string QuestionText => _question;

    public void GenerateId(int id)
    {
        _id = $"Quest{id+1}";
    }
}
public enum QuestionType
{
    Merge_Slimes,
    Reach_Score,
    Best_Score,
    Reach_Combo
}