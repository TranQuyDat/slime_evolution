using System;
using System.Collections.Generic;
using UnityEngine;

class SaveSystem
{
    public ISaveProvider Provider;
    public void Save<T>(T data , string key)
    {
        Provider.Save(data,key);
    }

    public T Load<T>(string key)
    {
        return Provider.Load<T>(key);
    }

    public void Delete(string key)
    {
        Provider.Delete(key);
    }
}