using UnityEngine;

public class GroundDetector
{
    private readonly Transform _transform;

    public GroundDetector(Transform transform)
    {
        _transform = transform;
    }

    public bool IsGrounded(LayerMask selfMask, Vector3 offset, float sphereRadius)
    {
        Vector3 sphereCenter = _transform.position + offset;

        return Physics.CheckSphere(sphereCenter, sphereRadius, ~selfMask, QueryTriggerInteraction.Ignore);
    }
}