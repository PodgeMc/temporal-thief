using UnityEngine;

public class CollectibleResetManager : MonoBehaviour
{
    CollectibleItem[] collectibles;

    void Awake()
    {
        collectibles = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None);
    }

    public void ResetAllCollectibles()
    {
        foreach (var collectible in collectibles)
        {
            if (collectible != null)
                collectible.ResetCollectible();
        }
    }
}
