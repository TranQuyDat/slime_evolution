using UnityEngine;

static class EditorSaveData
{
    public static void Save(string key , object value)
    {
        var json = JsonUtility.ToJson(value);
        UnityEditor.EditorPrefs.SetString(key, json);
    }
    public static void Load(string key, object target)
    {
        if (UnityEditor.EditorPrefs.HasKey(key))
        {
            string json = UnityEditor.EditorPrefs.GetString(key);
            JsonUtility.FromJsonOverwrite(json, target);
        }
    }
}