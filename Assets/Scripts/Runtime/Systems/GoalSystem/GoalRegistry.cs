using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoalRegistry",
    menuName = "Data/Goal Registry")]
class GoalRegistry : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        [HideInInspector] public string goalId;
        [SerializeField] private string goalIdDisplay ;
        public GoalData goalData;

        public void LoadGoalIdDisplay()
        {
            if(string.IsNullOrEmpty(goalIdDisplay) || goalIdDisplay != goalId)
                goalIdDisplay = goalId;
        }
    }

    [SerializeField] private List<Entry> _entries = new();

    public IReadOnlyList<Entry> Entries => _entries;

    void OnValidate()
    {
        if (_entries == null || _entries.Count == 0) return;

        string preGoalId = null;
        foreach (Entry entry in _entries)
        {
            if (string.IsNullOrEmpty(entry.goalId) 
            || entry.goalId == preGoalId)
            {
                entry.goalId = GetNextGoalId(entry);
            }

            if(entry.goalData != null && entry.goalData.GoalId != entry.goalId)
            {
                entry.goalData.SetGoalId(entry.goalId);
            }

            entry.LoadGoalIdDisplay();

            
            
            preGoalId = entry.goalId;
        }
    }


    private string GetNextGoalId(Entry currentEntry)
    {
        int index = 0;

        while (true)
        {
            string candidate = $"Goal_{index}";
            bool isUsed = false;

            foreach (Entry entry in _entries)
            {
                if (entry != currentEntry && entry.goalId == candidate)
                {
                    isUsed = true;
                    break;
                }
            }

            if (!isUsed)
                return candidate;

            index++;
        }
    }
    #if UNITY_EDITOR
    [ContextMenu("Save")]
    private void Save() => EditorSaveData.Save("GoalRegistry", this );
    [ContextMenu("Load")]
    private void Load()=> EditorSaveData.Load("GoalRegistry", this);
    #endif
}
    