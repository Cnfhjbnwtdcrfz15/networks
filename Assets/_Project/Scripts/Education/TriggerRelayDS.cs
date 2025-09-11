using UnityEngine;
using UnityEngine.Events;

public class TriggerRelay : MonoBehaviour
{
    [SerializeField] private bool _destroyAfterTrigger = true;
    [SerializeField] private string _requiredTag = "GameController";
    [SerializeField] private UnityEvent _onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_requiredTag))
        {
            _onTriggerEnter?.Invoke();

            if (_destroyAfterTrigger)
                Destroy(gameObject);
        }
    }
}