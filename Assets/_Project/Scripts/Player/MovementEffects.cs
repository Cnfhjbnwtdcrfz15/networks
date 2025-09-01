using UnityEngine;

namespace MiktoGames
{
    public class MovementEffects : MonoBehaviour
    {
        [Header("Эффекты передвижения")]
        [SerializeField] private PlayerController _player;
        [SerializeField, Range(0.05f, 2)] private float _rotationAmount = 0.2f;
        [SerializeField, Range(1f, 20)] private float _rotationSmooth = 6f;

        [Header("Педвижение")]
        [SerializeField] private bool _canMovementFX = true;
        [SerializeField, Range(0.1f, 2)] private float _movementAmount = 0.5f;
        
        private Quaternion _installRotation;
        private Vector3 _movementVector;

        private void Start()
        {
            _player = GetComponentInParent<PlayerController>();
            _installRotation = transform.localRotation;
        }

        private void Update()
        {
            float movementX = (_player.Vertical * _rotationAmount);
            float movementZ = (-_player.Horizontal * _rotationAmount);
            _movementVector = new Vector3(_canMovementFX ? movementX + _player.CharacterController.velocity.y * _movementAmount : movementX, 0, movementZ);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(_movementVector + _installRotation.eulerAngles), Time.deltaTime * _rotationSmooth);
        }
    }
}