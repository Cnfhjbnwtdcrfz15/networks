using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class MainPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Text")]
        public bool useCustomText = false;
        [SerializeField]
        private CustomText[] _textList =
        {
            new("TextEng", LangType.eng),
            new("TextRu", LangType.ru)
        };

        [Header("Icon")]
        public bool hasIcon = false;
        public Sprite iconSprite;

        [Header("Resources")]
        public Animator buttonAnimator;
        public TextMeshProUGUI normalText;
        public TextMeshProUGUI highlightedText;
        public TextMeshProUGUI pressedText;
        public Image normalIcon;
        public Image highlightedIcon;
        public Image pressedIcon;

        void OnEnable()
        {
            LangSwitcher.Changed += OnLangChange;

            if (buttonAnimator == null)
                buttonAnimator = gameObject.GetComponent<Animator>();

            UpdateTexts(LangSwitcher.Lang);

            if (hasIcon == true)
            {
                if (normalIcon != null) { normalIcon.sprite = iconSprite; }
                if (highlightedIcon != null) { highlightedIcon.sprite = iconSprite; }
                if (pressedIcon != null) { pressedIcon.sprite = iconSprite; }
            }

            else if (hasIcon == false)
            {
                if (normalIcon != null) { Destroy(normalIcon.gameObject); }
                if (highlightedIcon != null) { Destroy(highlightedIcon.gameObject); }
                if (pressedIcon != null) { Destroy(pressedIcon.gameObject); }
            }
        }

        private void OnDisable()
        {
            LangSwitcher.Changed -= OnLangChange;
        }

        private void OnLangChange(LangType type)
        {
            UpdateTexts(type);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Dissolve to Normal");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Normal to Dissolve");
        }

        private void UpdateTexts(LangType type)
        {
            if (useCustomText == false) return;

            var textForLang = Array.Find(_textList, t => t.СompareLang(type));
            if (textForLang == null) return;

            if (normalText != null) normalText.text = textForLang.Text;
            if (highlightedText != null) highlightedText.text = textForLang.Text;
            if (pressedText != null) pressedText.text = textForLang.Text;
        }
    }
}