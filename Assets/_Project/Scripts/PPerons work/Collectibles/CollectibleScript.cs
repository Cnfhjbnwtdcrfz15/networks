using System;
using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    Ray ray;
    public static Action PickCollectible;
    void Start()
    {
        ray = new Ray(transform.position, transform.forward);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireSkreenRay();
        }
    }

    private void FireSkreenRay()
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Collectible"))
            {
                PickCollectible?.Invoke();
                Destroy(hit.collider.gameObject);                
            }
            
        }
    }
}
