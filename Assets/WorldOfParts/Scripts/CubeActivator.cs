using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CubeActivator : MonoBehaviour
{
    private SphereCollider _collider;

    private void Awake() =>
        _collider = GetComponent<SphereCollider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Cube cube))
            cube.Activate(transform, _collider.radius);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Cube cube))
            cube.Deactivate();
    }
}