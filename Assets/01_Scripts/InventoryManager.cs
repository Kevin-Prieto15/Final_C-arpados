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

        AddItem("lance"); // owo
        AddItem("pickaxe");
    }

    public void AddItem(string itemName)
    {
        if (!inventory.ContainsKey(itemName))
            inventory[itemName] = 0;

        inventory[itemName]++;
        inventoryUI.UpdateUI(inventory);
        inventoryUI.CheckForSpecialItem(itemName);
    }

}
