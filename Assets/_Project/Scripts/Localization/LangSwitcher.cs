using System;
using UnityEngine;
using UnityEngine.UI;

public enum LangType
{
    ru,
    eng
}

public static class LangSwitcher
{
    public static event System.Action<LangType> Changed;

    public static LangType Lang => Saver.Lang;

    public static void Switch(LangType lang)
    {
        Saver.UpdateLang(lang);
        Saver.Save();
        Changed?.Invoke(lang);
        UpdateCanvas();
    }

    private static void UpdateCanvas()
    {
        Canvas.ForceUpdateCanvases();

        foreach (var layout in UnityEngine.Object.FindObjectsByType<LayoutGroup>(FindObjectsSortMode.None))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
        }
    }

    public static void Toggle()
    {
        if (Saver.Lang == LangType.eng)
            Switch(LangType.ru);
        else Switch(LangType.eng);
    }
}

[Serializable]
public class CustomText
{
    [SerializeField] private string _text;
    [SerializeField] private LangType _type;

    public CustomText(string text, LangType type)
    {
        _text = text;
        _type = type;
    }

    public string Text => _text;

    public bool ÑompareLang(LangType type) =>
        _type == type;
}

[Serializable]
public class CustomDescription : CustomText
{
    [SerializeField, TextArea] private string _description;

    public CustomDescription(string text, string description, LangType type)
        : base(text, type)
    {
        _description = description;
    }

    public string Description => _description;
}

[Serializable]
public class Subtitles : CustomText
{
    [SerializeField] private AudioClip _clip;

    public Subtitles(string text, AudioClip clip, LangType type)
        : base(text, type)
    {
        _clip = clip;
    }

    public AudioClip Clip => _clip;
}

[Serializable]
public class ResultSubtitles
{
    [SerializeField]
    private Subtitles[] _subtitles =
    {
        new("Subtitle", null, LangType.eng),
        new("Ñóáòèòðû", null, LangType.ru)
    };

    public string Text
    {
        get
        {
            LangType currentLang = LangSwitcher.Lang;

            for (int i = 0; i < _subtitles.Length; i++)
            {
                if (_subtitles[i].ÑompareLang(currentLang))
                    return _subtitles[i].Text;
            }

            return string.Empty;
        }
    }

    public AudioClip Clip
    {
        get
        {
            LangType currentLang = LangSwitcher.Lang;

            for (int i = 0; i < _subtitles.Length; i++)
            {
                if (_subtitles[i].ÑompareLang(currentLang))
                    return _subtitles[i].Clip;
            }

            return null;
        }
    }
}