using UnityEngine;

public class ElevatorCaller : MonoBehaviour
{
    [SerializeField] private Elevator _elevator;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out PlayerCollider _))
            _elevator.SetA();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out PlayerCollider _))
            _elevator.SetB();
    }
}