using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public void Init()
    {
    }
    public void SaveToJson<T>(T data, string path)
    {
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(path, jsonData);
    }

    public T LoadFromJson<T>(string path)
    {
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(jsonData);
        }
        else
        {
            Debug.LogError("File not found: " + path);
            return default(T);
        }
    }
}
