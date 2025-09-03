using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Shift
{
    public class LocalizationSwitcher : MonoBehaviour
    {
        // Константы
        private const string SaveKeySuffix = "HSelectorValue";

        // Поля
        [Header("Settings")]
        [SerializeField] private int _defaultIndex;
        [SerializeField] private bool _invokeAtStart;
        [SerializeField] private bool _invertAnimation;
        [SerializeField] private bool _loopSelection;
        [HideInInspector] public int Index;

        [Header("Saving")]
        [SerializeField] private bool _saveValue;
        [SerializeField] private string _selectorTag = "Tag Text";

        [Header("Indicators")]
        [SerializeField] private bool _enableIndicators = true;
        [SerializeField] private Transform _indicatorParent;
        [SerializeField] private GameObject _indicatorObject;

        [Header("Items")]
        [SerializeField] private List<Item> _itemList = new List<Item>();

        private TextMeshProUGUI _label;
        private TextMeshProUGUI _labelHelper;
        private Animator _selectorAnimator;

        // Вложенные типы
        [Serializable]
        public class Item
        {
            public string ItemTitle = "Item Title";
            public LangType LangType = LangType.eng;
            public UnityEvent OnValueChanged = new UnityEvent();
        }

        // Публичные геттеры для совместимости со старым кодом
        public List<Item> itemList => _itemList;
        public int index
        {
            get => Index;
            set => Index = value;
        }

        // Unity методы
        private void Start()
        {
            _selectorAnimator = GetComponent<Animator>();
            _label = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            _labelHelper = transform.Find("Text Helper").GetComponent<TextMeshProUGUI>();

            Index = LoadIndex();

            _label.text = _itemList[Index].ItemTitle;
            _labelHelper.text = _label.text;

            if (_enableIndicators)
                CreateIndicators();
            else
                Destroy(_indicatorParent);

            if (_invokeAtStart)
                _itemList[Index].OnValueChanged.Invoke();
        }

        // Публичные методы
        public void PreviousClick()
        {
            if (_loopSelection == false && Index == 0)
                return;

            _labelHelper.text = _label.text;

            if (Index == 0)
                Index = _itemList.Count - 1;
            else
                Index--;

            ApplySelection(_invertAnimation ? "Forward" : "Previous");
        }

        public void ForwardClick()
        {
            if (_loopSelection == false && Index == _itemList.Count - 1)
                return;

            _labelHelper.text = _label.text;

            if ((Index + 1) >= _itemList.Count)
                Index = 0;
            else
                Index++;

            ApplySelection(_invertAnimation ? "Previous" : "Forward");
        }

        public void CreateNewItem(string title)
        {
            Item item = new Item { ItemTitle = title };
            _itemList.Add(item);
        }

        public void UpdateUI()
        {
            _label.text = _itemList[Index].ItemTitle;

            if (_enableIndicators)
                CreateIndicators();
        }

        // Приватные методы
        private void ApplySelection(string animationName)
        {
            _label.text = _itemList[Index].ItemTitle;

            try
            {
                _itemList[Index].OnValueChanged.Invoke();
                LangSwitcher.Switch(_itemList[Index].LangType);
            }
            catch
            {
                // Игнорируем ошибки вызова
            }

            _selectorAnimator.Play(null);
            _selectorAnimator.StopPlayback();
            _selectorAnimator.Play(animationName);

            if (_enableIndicators)
                UpdateIndicators();
        }

        private void CreateIndicators()
        {
            foreach (Transform child in _indicatorParent)
                Destroy(child.gameObject);

            for (int i = 0; i < _itemList.Count; i++)
            {
                GameObject go = Instantiate(_indicatorObject, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(_indicatorParent, false);
                go.name = _itemList[i].ItemTitle;

                Transform onObj = go.transform.Find("On");
                Transform offObj = go.transform.Find("Off");

                bool isActive = i == Index;
                onObj.gameObject.SetActive(isActive);
                offObj.gameObject.SetActive(isActive == false);
            }
        }

        private void UpdateIndicators()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                GameObject go = _indicatorParent.GetChild(i).gameObject;
                Transform onObj = go.transform.Find("On");
                Transform offObj = go.transform.Find("Off");

                bool isActive = i == Index;
                onObj.gameObject.SetActive(isActive);
                offObj.gameObject.SetActive(isActive == false);
            }
        }

        private int LoadIndex()
        {
            LangType currentLang = LangSwitcher.Lang;

            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].LangType == currentLang)
                    return i;
            }

            return _defaultIndex;
        }
    }
}