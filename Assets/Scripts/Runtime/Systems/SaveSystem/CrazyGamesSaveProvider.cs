using System;
using System.Globalization;
using CrazyGames;
using UnityEngine;

class CrazyGamesSaveProvider : ISaveProvider
{
    public void Save<T>(T data, string key)
    {
        string value = typeof(T).IsPrimitive || typeof(T) == typeof(string)
            ? Convert.ToString(data, CultureInfo.InvariantCulture)
            : JsonUtility.ToJson(data);

        CrazySDK.Data.SetString(key, value);

#if UNITY_EDITOR
        // CrazySDK.Data falls back to PlayerPrefs while testing in Editor.
        PlayerPrefs.Save();
#endif
    }

    public T Load<T>(string key)
    {
        if (!CrazySDK.Data.HasKey(key)) return default;

        string value = CrazySDK.Data.GetString(key);
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return (T)Convert.ChangeType(
                value,
                typeof(T),
                CultureInfo.InvariantCulture);
        }

        return JsonUtility.FromJson<T>(value);
    }

    public void Delete(string key)
    {
        CrazySDK.Data.DeleteKey(key);
    }
}
