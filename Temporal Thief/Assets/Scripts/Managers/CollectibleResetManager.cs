using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleResetManager : MonoBehaviour
{
    private CollectibleItem[] collectibles;

    void Awake()
    {
        collectibles = FindObjectsOfType<CollectibleItem>(true);
    }

    public void ResetAllCollectibles()
    {
        foreach (CollectibleItem item in collectibles)
        {
            if (item != null)
                item.ResetCollectible();
        }

        Debug.Log("All collectibles reset.");
    }
}