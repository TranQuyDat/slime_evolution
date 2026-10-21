using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using UnityEngine.InputSystem.Utilities;

class GoalSystem 
{
    private GoalData[] _goalDatas;
    private List<(string goalId,Question quest)> _selectedQuests = new();


    public void Initialize()
    {
        _goalDatas = Resources.LoadAll<GoalData>("Data");

        var Achivements = _goalDatas
        .Where(g => g.GoalType == GoalData.Type.Achievement)
        .SelectMany(
            goal => goal.Questions  ?? Array.Empty<Question>(),
            (goal,question) => new { Goal = goal , Question = question}
        );

        foreach(var item in Achivements)
        {
            var select = (item.Goal.GoalId,item.Question);
            _selectedQuests.Add(select);
        }
        
        
        
    }
    

    public void AddRandomMatch(GoalData.Difficulty difficulty)
    {
        var matches = _goalDatas
            .Where(goal => goal != null && 
                        goal.GoalType == GoalData.Type.Match && 
                        goal.GoalDifficulty == difficulty)
            // Truyền cả goal và question xuống bước tiếp theo
            .SelectMany(
                goal => goal.Questions ?? Array.Empty<Question>(),
                (goal, question) => new { Goal = goal, Question = question }
            )
            .Where(item => item.Question != null)
            .ToArray();

        if (matches.Length == 0)
        {
            Debug.LogWarning($"[GoalSystem] No {difficulty} match question was found.");
            return ;
        }
        
        var randomMatch = matches[UnityEngine.Random.Range(0, matches.Length)];
        _selectedQuests.Add(($"{randomMatch.Goal.GoalId}", randomMatch.Question));
    }

    public void RemoveQuest(GoalData.Type type)
    {
        for(int i = _selectedQuests.Count-1 ; i>=0 ;i--)
        {
            var item  =  _selectedQuests[i];
            var goal = GetGoalDataById(item.goalId);
            if(goal.GoalType != type) continue;
            _selectedQuests.RemoveAt(i);
        }
    }
    public (string goalId,Question quest)[] GetQuests(GoalData.Type type)
    {
        return _selectedQuests
        .Where(i => GetGoalDataById(i.goalId).GoalType == type)
        .ToArray();
    }


    public void UpdateProgress( QuestionType type,
        float value )
    {
        var saveSystem = GameManager.Instance.SaveSystem;

        var result = _selectedQuests
            .Where(q => q.quest.Type == type)
            .ToArray();


        foreach (var r in result)
        {

            var goalData = GetGoalDataById(r.goalId);
            //save achievement progress
            if(goalData.GoalType != GoalData.Type.Achievement) continue;

            if(type == QuestionType.Best_Score)
            {
                saveSystem.Save<float>(value,$"hightscore");
                return;
            }
            
            saveSystem.Save<float>(value, 
            $"{r.goalId}_{r.quest.Id}");
        }
    }
            
    public GoalData GetGoalDataById(string id)
    {
        return _goalDatas.FirstOrDefault(goal => goal.GoalId == id);
    }

}
