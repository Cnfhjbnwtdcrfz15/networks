using System;
using TMPro;
using UnityEngine;

public class CustomTMP : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    [SerializeField]
    private CustomText[] _textList =
        {
            new("TextEng", LangType.eng),
            new("TextRu", LangType.ru)
        };

    void OnEnable()
    {
        LangSwitcher.Changed += OnLangChange;
        UpdateTexts(LangSwitcher.Lang);
    }
    void OnDisable()
    {
        LangSwitcher.Changed -= OnLangChange;
    }

    private void UpdateTexts(LangType type)
    {
        var textForLang = Array.Find(_textList, t => t.ÑompareLang(type));
        if (textForLang == null) return;

        _text.text = textForLang.Text;
    }

    private void OnLangChange(LangType type)
    {
        UpdateTexts(type);
    }
}