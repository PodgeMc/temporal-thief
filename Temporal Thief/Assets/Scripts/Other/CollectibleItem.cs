using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Key";

    public AudioClip collectSound;
    [Range(0f, 1f)] public float volume = 1f;

    private bool collected = false;
    private Collider col;
    private Renderer[] rends;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;

        rends = GetComponentsInChildren<Renderer>(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (!other.CompareTag("Player") && !other.CompareTag("Echo"))
            return;

        collected = true;

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddItem(itemName);
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }

        Debug.Log(other.tag + " collected: " + itemName);

        SetVisible(false);
    }

    public void ResetCollectible()
    {
        collected = false;
        SetVisible(true);
    }

    void SetVisible(bool value)
    {
        if (col != null)
            col.enabled = value;

        foreach (Renderer r in rends)
        {
            if (r != null)
                r.enabled = value;
        }
    }
}