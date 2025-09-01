using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MiktoGames
{
    public class ItemChange : MonoBehaviour
    {
        [Header("Смена предметов")]
        public Animator _animator;
        [SerializeField] private Image _itemCanvasLogo;
        [SerializeField] private bool _loopItems = true;
        [SerializeField, Tooltip("Ты можешь добавить свой предмет сюда.")] private GameObject[] _items;
        [SerializeField, Tooltip("Эти логотипы должны иметь тот же порядок, что и предметы..")] private Sprite[] _itemLogos;
        [SerializeField] private int ItemIdInt;

        private int _maxItems;
        private int _changeItemInt;

        public bool _definiteHide;

        private bool _itemChangeLogo;

        private void Start()
        {
            if (_animator == null && GetComponent<Animator>()) _animator = GetComponent<Animator>();
            Color OpacityColor = _itemCanvasLogo.color;
            OpacityColor.a = 0;
            _itemCanvasLogo.color = OpacityColor;
            _itemChangeLogo = false;
            _definiteHide = false;
            _changeItemInt = ItemIdInt;
            _itemCanvasLogo.sprite = _itemLogos[ItemIdInt];
            _maxItems = _items.Length - 1;
            StartCoroutine(ItemChangeObject());
        }
        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                ItemIdInt++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                ItemIdInt--;
            }

            if(Input.GetKeyDown(KeyCode.H))
            {
                if (_animator.GetBool("Hide")) Hide(false);
                else Hide(true);
            }

            if (ItemIdInt < 0) ItemIdInt = _loopItems ? _maxItems : 0;
            if (ItemIdInt > _maxItems) ItemIdInt = _loopItems ? 0 : _maxItems;


            if (ItemIdInt != _changeItemInt)
            {
                _changeItemInt = ItemIdInt;
                StartCoroutine(ItemChangeObject());
            }
        }

        public void Hide(bool Hide)
        {
            _definiteHide = Hide;
            _animator.SetBool("Hide", Hide);
        }

        IEnumerator ItemChangeObject()
        {
            if(!_definiteHide) _animator.SetBool("Hide", true);
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < (_maxItems + 1); i++)
            {
                _items[i].SetActive(false);
            }
            _items[ItemIdInt].SetActive(true);
            if (!_itemChangeLogo) StartCoroutine(ItemLogoChange());

            if (!_definiteHide) _animator.SetBool("Hide", false);
        }

        IEnumerator ItemLogoChange()
        {
            _itemChangeLogo = true;
            yield return new WaitForSeconds(0.5f);
            _itemCanvasLogo.sprite = _itemLogos[ItemIdInt];
            yield return new WaitForSeconds(0.1f);
            _itemChangeLogo = false;
        }

        private void FixedUpdate()
        {
            
            if (_itemChangeLogo)
            {
                Color OpacityColor = _itemCanvasLogo.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, 20 * Time.deltaTime);
                _itemCanvasLogo.color = OpacityColor;
            }
            else
            {
                Color OpacityColor = _itemCanvasLogo.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, 6 * Time.deltaTime);
                _itemCanvasLogo.color = OpacityColor;
            }
        }
    }

}
