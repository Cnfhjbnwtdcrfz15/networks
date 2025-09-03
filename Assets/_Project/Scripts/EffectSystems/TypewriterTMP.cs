using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterTMP : MonoBehaviour
{
    [SerializeField] private TMP_Text _textUI;
    public float CharDelay = 0.05f;

    private Coroutine _typingCoroutine;
    private string _fullText;
    private bool _isTyping;

    public void ShowText(string text, float delay = -1f)
    {
        if (delay > 0) CharDelay = delay;

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _fullText = text;
        _typingCoroutine = StartCoroutine(TypeText());
    }
    
    public void ShowFullText()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _textUI.text = _fullText;
        _isTyping = false;
    }

    private IEnumerator TypeText()
    {
        _isTyping = true;
        _textUI.text = "";

        foreach (char c in _fullText)
        {
            _textUI.text += c;
            yield return new WaitForSeconds(CharDelay);
        }

        _isTyping = false;
    }

    public bool IsTyping()
    {
        return _isTyping;
    }
}