using UnityEngine;

public class TriggerRelayDS : MonoBehaviour
{
    [SerializeField] private DialogueSystem _tutorialManager;
    [SerializeField] private int _stepIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            _tutorialManager.StartStep(_stepIndex);
        }

        Destroy(gameObject);
    }
}