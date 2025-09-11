using UnityEngine;

public class Rotator
{
    private readonly Transform _character;
    private readonly Transform _head;
    private Vector2 _verticalLimits = new(-90, 90);

    private float _yaw;
    private float _pitch;

    public Rotator(Transform character, Transform head)
    {
        _character = character;
        _head = head;
    }

    public void Rotate(Vector2 inputDelta, float sensitivity)
    {
        if (inputDelta == Vector2.zero)
            return;

        _yaw += inputDelta.x * sensitivity;
        _pitch -= inputDelta.y * sensitivity;

        _pitch = Mathf.Clamp(_pitch, _verticalLimits.x, _verticalLimits.y);

        _character.localRotation = Quaternion.Euler(0f, _yaw, 0f);

        if (_head != null)
            _head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}