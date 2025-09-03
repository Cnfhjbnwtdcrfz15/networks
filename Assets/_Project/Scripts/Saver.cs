using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public LangType Lang = LangType.eng;
}

public static class Saver
{
    private static SaveData _data;
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static LangType Lang => _data.Lang;

    static Saver()
    {
        Load();
    }

    public static void UpdateLang(LangType lang)
    {
        _data.Lang = lang;
    }

    public static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[Saver] Данные сохранены в {SavePath}:\n{json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Saver] Ошибка сохранения: {e}");
        }
    }

    private static void Load()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                _data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"[Saver] Данные загружены из {SavePath}:\n{json}");
            }
            else
            {
                _data = new SaveData();
                Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Saver] Ошибка загрузки: {e}");
            _data = new SaveData();
        }
    }
}