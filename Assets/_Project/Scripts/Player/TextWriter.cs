using UnityEngine;
using UnityEngine.UI;

namespace MiktoGames
{
    public class TextWriter : MonoBehaviour
    {
        [Header("Отображение текстов")]
        [SerializeField] private Text _textElement;
        [SerializeField] private string _textToWrite;
        [SerializeField] private float _timeToWrite;
        private float _timer;
        private int _index;
        private bool _writing;

        public void AddWriter(string TextToWrite, Text TextElement, float TimeToWrite)
        {
            this._textToWrite = TextToWrite;
            this._timeToWrite = TimeToWrite;
            this._textElement = TextElement;
            _index = 0;
        }

        void Update()
        {
            if (_textElement != null)
            {
                _timer -= Time.deltaTime;
                while (_timer < 0f)
                {
                    _timer += _timeToWrite;
                    _index++;
                    _textElement.text = _textToWrite.Substring(0, _index);

                    if (_index >= _textToWrite.Length)
                    {
                        _textElement = null;
                        Debug.Log("Конец текста");

                        return;
                    }
                }
            }
        }
    }
}