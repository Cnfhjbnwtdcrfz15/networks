using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    [Header("Jump Settings")]
    [SerializeField] private LayerMask _selfLayer;
    [SerializeField] private Vector3 _groundCheckOffset = new(0, 0.39f, 0);
    [SerializeField] private float _groundSphereRadius = 0.4f;
    [SerializeField] private float _jumpVelocity = 500f;
    [SerializeField] private float _gravityMultiplier = 2f;

    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 7f;
    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _crouchSpeed = 4f;

    [Header("Look Settings")]
    [SerializeField] private float _mouseSensitivity = 10f;

    private Character _character;
    private bool _isSprinting = false;
    private bool _isCrouching = false;

    private void Awake() =>
        _character = new(transform, Camera.main.transform, GetComponent<Rigidbody>());

    private void FixedUpdate() =>
        _character.ApplyExtraGravity(_gravityMultiplier);

    private void OnEnable()
    {
        _inputReader.MovementPressed += OnMovement;
        _inputReader.RotationPressed += OnRotation;
        _inputReader.JumpPressed += OnJump;
        _inputReader.SprintPressed += OnSprintStart;
        _inputReader.SprintUnpressed += OnSprintStop;
        _inputReader.CrouchPressed += OnCrouchStart;
        _inputReader.CrouchUnpressed += OnCrouchStop;
    }

    private void OnDisable()
    {
        _inputReader.MovementPressed -= OnMovement;
        _inputReader.RotationPressed -= OnRotation;
        _inputReader.JumpPressed -= OnJump;
        _inputReader.SprintPressed -= OnSprintStart;
        _inputReader.SprintUnpressed -= OnSprintStop;
        _inputReader.CrouchPressed -= OnCrouchStart;
        _inputReader.CrouchUnpressed -= OnCrouchStop;
    }

    private void OnMovement(Vector2 direction)
    {
        float speed;

        if (_isCrouching)
            speed = _crouchSpeed;
        else if (_isSprinting)
            speed = _sprintSpeed;
        else
            speed = _walkSpeed;

        _character.Move(direction, speed);
    }

    private void OnRotation(Vector2 delta) =>
        _character.Rotate(delta, _mouseSensitivity);

    private void OnJump() =>
        _character.Jump(_jumpVelocity, _selfLayer, _groundCheckOffset, _groundSphereRadius);

    private void OnSprintStart() =>
        _isSprinting = true;

    private void OnSprintStop() =>
        _isSprinting = false;

    private void OnCrouchStart() =>
        _isCrouching = true;

    private void OnCrouchStop() =>
        _isCrouching = false;
}