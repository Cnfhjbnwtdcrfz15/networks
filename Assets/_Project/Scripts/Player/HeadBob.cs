using UnityEngine;

namespace MiktoGames
{
    [RequireComponent(typeof(Camera))]
    public class HeadBob : MonoBehaviour
    {
        [Header("Эффект HeadBob")]
        [SerializeField] private bool _enabled = true;

        [Space, Header("Основное")]
        [SerializeField, Range(0.001f, 0.01f)] private float _amount = 0.00484f;
        [SerializeField, Range(10f, 30f)] private float _frequency = 16.0f;
        [SerializeField, Range(100f, 10f)] private float _smooth = 44.7f;

        [Header("Вращение/Движение")]
        [SerializeField] private bool _enabledRoationMovement = true;
        [SerializeField, Range(40f, 4f)] private float _roationMovementSmooth = 10.0f;
        [SerializeField, Range(1f, 10f)] private float _roationMovementAmount = 3.0f;

        private float _toggleSpeed = 3.0f;

        private Vector3 _startPos;
        private Vector3 _startRot;
        private Vector3 _finalRot;

        private CharacterController _player;

        private void Awake()
        {
            _player = GetComponentInParent<CharacterController>();
            _startPos = transform.localPosition;
            _startRot = transform.localRotation.eulerAngles;
        }

        private void Update()
        {
            if (!_enabled) return;
            CheckMotion();
            ResetPos();
            if (_enabledRoationMovement) transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(_finalRot), _roationMovementSmooth * Time.deltaTime);
        }

        private void CheckMotion()
        {
            float speed = new Vector3(_player.velocity.x, 0, _player.velocity.z).magnitude;
            if (speed < _toggleSpeed) return;
            if (!_player.isGrounded) return;
            PlayMotion(HeadBobMotion());
        }

        private void PlayMotion(Vector3 Movement)
        {
            transform.localPosition += Movement;
            _finalRot += new Vector3(-Movement.x, -Movement.y, Movement.x) * _roationMovementAmount;
        }

        private Vector3 HeadBobMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * _frequency) * _amount * 1.4f, _smooth * Time.deltaTime);
            pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * _frequency / 2f) * _amount * 1.6f, _smooth * Time.deltaTime);
            return pos;
        }

        private void ResetPos()
        {
            if (transform.localPosition == _startPos) return;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startPos, 1 * Time.deltaTime);
            _finalRot = Vector3.Lerp(_finalRot, _startRot, 1 * Time.deltaTime);
        }
    }
}