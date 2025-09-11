using UnityEngine;

public class Jumper
{
    private readonly Rigidbody _rigidbody;

    public Jumper(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
    }

    public void Jump(float velocity) =>
        _rigidbody.AddForce(velocity * Vector3.up, ForceMode.Impulse);
}