using UnityEngine;

class PlayerPrefsProvider : ISaveProvider
{
    public void Save<T>(T data, string key)
    {
        string json  = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key,json);
        PlayerPrefs.Save();
    }
    public T Load<T>(string key)
    {
        if(!PlayerPrefs.HasKey(key)) return default;
        string json  = PlayerPrefs.GetString(key);
        T t = JsonUtility.FromJson<T>(json);
        return t;
    }

    public void Delete(string key)
    {
        if(PlayerPrefs.HasKey(key))
            PlayerPrefs.DeleteKey(key);
    }

}