using UnityEngine;
using UnityEngine.UI;

namespace MiktoGames
{
    public class Viewpoint : MonoBehaviour
    {
        [Header("Viewpoint")]
        [SerializeField] private string _pointText = "ֽאזלטעו E";
        [Space, SerializeField] private Camera _cam;
        [SerializeField] private GameObject _playerController;
        [SerializeField] private Image _imagePrefab;
        [Space ,SerializeField, Range(0.1f, 20)] private float _maxViewRange = 8;
        [SerializeField, Range(0.1f, 20)] private float _maxTextViewRange = 3;
        private float _distance;
        private Text _imageText;
        private Image _imageUI;

        void Start()
        {
            _imageUI = Instantiate(_imagePrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
            _imageText = _imageUI.GetComponentInChildren<Text>();
            _imageText.text = _pointText;
        }

        void Update()
        {
            _imageUI.transform.position = _cam.WorldToScreenPoint(calculateWorldPosition(transform.position, _cam));
            _distance = Vector3.Distance(_playerController.transform.position, transform.position);

            if(_distance < _maxTextViewRange)
            {
                Color OpacityColor = _imageText.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, 10 * Time.deltaTime);
                _imageText.color = OpacityColor;
            }
            else
            {
                Color OpacityColor = _imageText.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, 10 * Time.deltaTime);
                _imageText.color = OpacityColor;
            }

            if (_distance < _maxViewRange)
            {
                Color OpacityColor = _imageUI.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, 10 * Time.deltaTime);
                _imageUI.color = OpacityColor;
            }
            else
            {
                Color OpacityColor = _imageUI.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, 10 * Time.deltaTime);
                _imageUI.color = OpacityColor;
            }
        }

        private Vector3 calculateWorldPosition(Vector3 position, Camera camera)
        {
            Vector3 camNormal = camera.transform.forward;
            Vector3 vectorFromCam = position - camera.transform.position;
            float camNormDot = Vector3.Dot(camNormal, vectorFromCam.normalized);
            if (camNormDot <= 0f)
            {
                float camDot = Vector3.Dot(camNormal, vectorFromCam);
                Vector3 proj = (camNormal * camDot * 1.01f);
                position = camera.transform.position + (vectorFromCam - proj);
            }

            return position;
        }
    }
}
