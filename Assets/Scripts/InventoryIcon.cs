using UnityEngine;

public class InventoryIcon : MonoBehaviour
{
    [Tooltip("Must match the key in Inventory.owned (case-insensitive)")]
    public string itemName;

    void Start()
    {
        Refresh();
    }

    // Call this after a purchase to update without reloading the scene.
    public void Refresh()
    {
        string key = itemName.ToLower();
        bool owned = Inventory.owned.ContainsKey(key) && Inventory.owned[key];
        gameObject.SetActive(owned);
    }
}
