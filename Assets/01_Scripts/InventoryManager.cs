using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    public InventoryUI inventoryUI;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(string itemName)
    {
        if (!inventory.ContainsKey(itemName))
            inventory[itemName] = 0;

        inventory[itemName]++;
        inventoryUI.UpdateUI(inventory);
        UpdateInventoryUI();
    }

    public Dictionary<string, int> GetInventory()
    {
        return inventory;
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUI != null)
            inventoryUI.UpdateUI(inventory);
    }

}
