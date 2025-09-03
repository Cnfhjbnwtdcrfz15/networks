using UnityEngine;

namespace MiktoGames
{
    public class HandsHolder : MonoBehaviour
    {
        [Header("Держатель предметов в руках")]
        [SerializeField] bool _enabled = true;

        [Space, Header("Основное")]
        [SerializeField, Range(0.0005f, 0.02f)] private float _amount = 0.005f;
        [SerializeField, Range(1.0f, 3.0f)] private float _sprintAmount = 1.4f;

        [SerializeField, Range(5f, 20f)] private float _frequency = 13.0f;
        [SerializeField, Range(50f, 10f)] private float _smooth = 24.2f;
        [Header("Вращение/Движение")]
        [SerializeField] private bool _enabledRotationMovement = true;
        [SerializeField, Range(0.1f, 10.0f)] private float _rotationMultipler = 6f;

        private float _toggleSpeed = 1.5f;
        private float _amountValue;

        private Vector3 _startPos;
        private Vector3 _startRot;
        private Vector3 _finalPos;
        private Vector3 _finalRot;

        private CharacterController _player;

        private void Awake()
        {
            _player = GetComponentInParent<CharacterController>();
            if (_player.transform.GetComponent<PlayerController>() != null) _toggleSpeed = _player.transform.GetComponent<PlayerController>().CroughSpeed * 1.5f;
            else _toggleSpeed = 1.5f;
            _amountValue = _amount;
            _startPos = transform.localPosition;
            _startRot = transform.localRotation.eulerAngles;
        }

        private void Update()
        {
            if (!_enabled) return;

            float speed = new Vector3(_player.velocity.x, 0, _player.velocity.z).magnitude;
            Reset();

            if (speed > _toggleSpeed && _player.isGrounded)
            {
                _finalPos += HeadBobMotion();
                _finalRot += new Vector3(-HeadBobMotion().z, 0, HeadBobMotion().x) * _rotationMultipler * 10;
            }
            else if (speed > _toggleSpeed) _finalPos += HeadBobMotion() / 2f;

            if (Input.GetKeyDown(KeyCode.LeftShift)) _amountValue = _amount * _sprintAmount;
            else if (Input.GetKeyUp(KeyCode.LeftShift)) _amountValue = _amount / _sprintAmount;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _finalPos, _smooth * Time.deltaTime);
            if (_enabledRotationMovement) transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(_finalRot), _smooth / 1.5f * Time.deltaTime);

        }

        private Vector3 HeadBobMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * _frequency) * _amountValue * 2f, _smooth * Time.deltaTime);
            pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * _frequency / 2f) * _amountValue * 1.3f, _smooth * Time.deltaTime);
            return pos;
        }
        private void Reset()
        {
            if (transform.localPosition == _startPos) return;
            _finalPos = Vector3.Lerp(_finalPos, _startPos, 1 * Time.deltaTime);
            _finalRot = Vector3.Lerp(_finalRot, _startRot, 1 * Time.deltaTime);
        }
    }
}