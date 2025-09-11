using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character
{
    private readonly Mover _mover;
    private readonly Rotator _rotator;
    private readonly Jumper _jumper;
    private readonly GroundDetector _groundDetector;
    private readonly Rigidbody _rigidbody;

    public Character(Transform character, Transform head, Rigidbody rigidbody)
    {
        _mover = new(character, rigidbody);
        _rotator = new(character, head);
        _jumper = new(rigidbody);
        _groundDetector = new(character);
        _rigidbody = rigidbody;
    }

    public void ApplyExtraGravity(float fallMultiplier) =>
        _rigidbody.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);

    public void Move(Vector2 direction, float speed) =>
        _mover.Move(direction, speed);

    public void Rotate(Vector2 delta, float sensitivity) =>
        _rotator.Rotate(delta, sensitivity);


    public void Jump(float velocity, LayerMask layerMask, Vector3 offset, float sphereRadius)
    {
        if (_groundDetector.IsGrounded(layerMask, offset, sphereRadius))
            _jumper.Jump(velocity);
    }
}