interface ISaveProvider
{
    void Save<T>(T data ,string key);
    T Load<T>(string key);
    void Delete(string key);
}