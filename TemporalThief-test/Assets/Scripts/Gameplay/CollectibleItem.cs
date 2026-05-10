using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Key";
    public AudioClip collectSound;
    [Range(0f, 1f)] public float volume = 1f;

    bool collected;
    Collider itemCollider;
    Renderer[] renderers;

    void Awake()
    {
        itemCollider = GetComponent<Collider>();
        itemCollider.isTrigger = true;
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player") && !other.CompareTag("Echo"))
            return;

        collected = true;
        PlayerInventory.Instance?.AddItem(itemName);

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);

        SetVisible(false);
    }

    public void ResetCollectible()
    {
        collected = false;
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        if (itemCollider != null)
            itemCollider.enabled = visible;

        foreach (var r in renderers)
            if (r != null)
                r.enabled = visible;
    }
}
