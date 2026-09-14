using System;
using UnityEngine;
class PlayerPrefsProvider : ISaveProvider
{
    public void Save<T>(T data, string key)
    {
        string json;
        if(typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            json = data.ToString();
        }
        else
            json  = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key,json);
        PlayerPrefs.Save();
    }
    public T Load<T>(string key)
    {
        if(!PlayerPrefs.HasKey(key)) return default;
        string json  = PlayerPrefs.GetString(key);
        if(typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return (T)Convert.ChangeType(json , typeof(T),
            System.Globalization.CultureInfo.InvariantCulture);
        }
        T t = JsonUtility.FromJson<T>(json);
        return t;
    }

    public void Delete(string key)
    {
        if(PlayerPrefs.HasKey(key))
            PlayerPrefs.DeleteKey(key);
    }

}