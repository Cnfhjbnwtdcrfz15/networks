using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Michsky.UI.Shift
{
    public class ChapterButton : MonoBehaviour
    {
        [Header("Resources")]
        public Sprite backgroundImage;
        [SerializeField]
        private CustomDescription[] _buttonTitleList =
        {
            new("TextEng", "Description", LangType.eng),
            new("TextRu", "Описание", LangType.ru)
        };

        [Header("Settings")]
        public bool useCustomResources = false;

        [Header("Status")]
        public bool enableStatus;
        public StatusItem statusItem;

        private Image backgroundImageObj;
        private TextMeshProUGUI titleObj;
        private TextMeshProUGUI descriptionObj;
        private Transform statusNone;
        private Transform statusLocked;
        private Transform statusCompleted;

        public enum StatusItem
        {
            None,
            Locked,
            Completed
        }

        private void OnEnable()
        {
            LangSwitcher.Changed += OnLangChange;

            InitResources();
            UpdateTexts(LangSwitcher.Lang);
            UpdateStatus();
        }

        private void OnDisable()
        {
            LangSwitcher.Changed -= OnLangChange;
        }

        private void InitResources()
        {
            if (useCustomResources) return;

            backgroundImageObj = transform.Find("Content/Background").GetComponent<Image>();
            titleObj = transform.Find("Content/Texts/Title").GetComponent<TextMeshProUGUI>();
            descriptionObj = transform.Find("Content/Texts/Description").GetComponent<TextMeshProUGUI>();

            if (backgroundImageObj != null)
                backgroundImageObj.sprite = backgroundImage;
        }

        private void OnLangChange(LangType type)
        {
            UpdateTexts(type);
        }

        private void UpdateTexts(LangType type)
        {
            var data = Array.Find(_buttonTitleList, t => t.СompareLang(type));
            if (data == null) return;

            if (titleObj != null) titleObj.text = data.Text;
            if (descriptionObj != null) descriptionObj.text = data.Description;
        }

        private void UpdateStatus()
        {
            if (!enableStatus) return;

            statusNone = transform.Find("Content/Texts/Status/None");
            statusLocked = transform.Find("Content/Texts/Status/Locked");
            statusCompleted = transform.Find("Content/Texts/Status/Completed");

            if (statusNone == null || statusLocked == null || statusCompleted == null) return;

            statusNone.gameObject.SetActive(statusItem == StatusItem.None);
            statusLocked.gameObject.SetActive(statusItem == StatusItem.Locked);
            statusCompleted.gameObject.SetActive(statusItem == StatusItem.Completed);
        }
    }
}