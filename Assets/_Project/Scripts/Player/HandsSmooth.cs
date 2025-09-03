using UnityEngine;

namespace MiktoGames
{
    public class HandsSmooth : MonoBehaviour
    {
        [Header("Ñãëàæèâàíèå ðóê")]
        [SerializeField] private CharacterController _charañterController;
        [SerializeField, Range(1, 10)] private float _smooth = 4f;
        [SerializeField, Range(0.001f, 1)] private float _amount = 0.03f;
        [SerializeField, Range(0.001f, 1)] private float _maxAmount = 0.04f;

        [Header("Ïîâîðîò")]
        [SerializeField, Range(1, 10)] private float _rotationSmooth = 4.0f;
        [SerializeField, Range(0.1f, 10)] private float _rotationAmount = 1.0f;
        [SerializeField, Range(0.1f, 10)] private float _maxRotationAmount = 5.0f;
        [SerializeField, Range(0.1f, 10)] private float _rotationMovementMultipler = 1.0f;

        [Header("Ïîâîðîò ñèäÿ")]
        [SerializeField] private bool _enabledCroughRotation = false;
        [SerializeField, Range(0.1f, 20)] private float _rotationCroughSmooth = 15.0f;
        [SerializeField, Range(5f, 50)] private float _rotationCroughMultipler = 18.0f;

        [Header("Èíïóòû")]
        [SerializeField] private KeyCode _croughKey = KeyCode.LeftControl;

        private float _croughRotation;
        private Vector3 _installPosition;
        private Quaternion _installRotation;

        private void Start()
        {
            _installPosition = transform.localPosition;
            _installRotation = transform.localRotation;
        }

        private void Update()
        {
            float InputX = -Input.GetAxis("Mouse X");
            float InputY = -Input.GetAxis("Mouse Y");
            float horizontal = -Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float moveX = Mathf.Clamp(InputX * _amount, -_maxAmount, _maxAmount);
            float moveY = Mathf.Clamp(InputY * _amount, -_maxAmount, _maxAmount);

            Vector3 finalPosition = new Vector3(moveX, moveY + -_charañterController.velocity.y / 60, 0);

            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + _installPosition, Time.deltaTime * _smooth);

            float TiltX = Mathf.Clamp(InputX * _rotationAmount, -_maxRotationAmount, _maxRotationAmount);
            float TiltY = Mathf.Clamp(InputY * _rotationSmooth, -_maxRotationAmount, _maxRotationAmount);
            if (_enabledCroughRotation && Input.GetKey(_croughKey)) _croughRotation = Mathf.Lerp(_croughRotation, _rotationCroughMultipler, _rotationCroughSmooth * Time.deltaTime);
            else _croughRotation = Mathf.Lerp(_croughRotation, 0f, _rotationCroughSmooth * Time.deltaTime);
            Vector3 vector = new Vector3(Mathf.Max(vertical * 0.4f, 0) * _rotationMovementMultipler, 0, horizontal * _rotationMovementMultipler);
            Vector3 finalRotation = new Vector3(-TiltY, 0, TiltX + _croughRotation) + vector;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(finalRotation) * _installRotation, Time.deltaTime * _rotationSmooth);
        }
    }
}