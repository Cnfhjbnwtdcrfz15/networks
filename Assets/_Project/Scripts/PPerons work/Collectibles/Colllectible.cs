using UnityEngine;

public class Colllectible : MonoBehaviour
{
    private int collectibleCount;
    public int collectibleLevelCount;

    void OnEnable()
    {
        CollectibleScript.PickCollectible += CollectiblePiked;
    }

    void OnDisable()
    {
        CollectibleScript.PickCollectible -= CollectiblePiked;
    }
    void Start()
    {
        collectibleCount = 0;
    }
    private void CollectiblePiked()
    {
        collectibleCount += 1;

        
        
        // if (collectibleCount >= collectibleLevelCount) {

        // }
    }
}
