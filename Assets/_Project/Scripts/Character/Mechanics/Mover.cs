using UnityEngine;

public class Mover
{
    private readonly Transform _transform;
    private readonly Rigidbody _rigidbody;

    public Mover(Transform characterTransform, Rigidbody rigidbody)
    {
        _transform = characterTransform;
        _rigidbody = rigidbody;
    }

    public void Move(Vector2 direction, float speed)
    {
        if(direction == Vector2.zero)
        {
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
            return;
        }

        direction = direction.normalized;
        Vector3 targetVelocity = _transform.TransformDirection(new Vector3(direction.x, 0f, direction.y)) * speed;
        targetVelocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = targetVelocity;
    }
}