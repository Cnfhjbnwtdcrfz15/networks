using UnityEngine;

namespace MiktoGames
{
    public class Pause : MonoBehaviour
    {
        [Header("Пауза")]
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerController _player;

        [Header("Input")]
        [SerializeField] private KeyCode _backKey = KeyCode.Escape;

        private void Update()
        {
            if (Input.GetKeyDown(_backKey))
            {
                if (_menuPanel.activeInHierarchy)
                {
                    _menuPanel.SetActive(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1.0f;
                    _animator.SetBool("START", false);
                }
                else
                {
                    _menuPanel.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0.0f;
                    _animator.SetBool("START", true);
                }
            }
        }
    }
}

   
