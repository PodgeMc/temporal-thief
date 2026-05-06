using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private List<string> items = new List<string>();
    public IReadOnlyList<string> Items => items;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return;

        items.Add(itemName);
        Debug.Log("Picked up: " + itemName);
    }

    public void ClearInventory()
    {
        items.Clear();
        Debug.Log("Inventory cleared.");
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}
