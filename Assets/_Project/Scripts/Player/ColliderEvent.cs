using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))] 
public class ColliderEvent : MonoBehaviour
{
    [Header("Коллайдер ивент")]
    [Space(20)]
    public string GameObjectTag = "GameController";
    [Space(10)]
    public UnityEvent Event;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == GameObjectTag)
            Event.Invoke();
    }
}