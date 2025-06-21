using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panel;
    public Text contentText;

    public Dropdown specialItemsDropdown; // o TMP_Dropdown si usas TextMeshPro
    public Button useSpecialItemButton;
    private List<string> specialItems = new List<string>();


    private void Start()
    {
        specialItemsDropdown.gameObject.SetActive(false);
        useSpecialItemButton.gameObject.SetActive(false);

        useSpecialItemButton.onClick.AddListener(UseSelectedSpecialItem);

        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Abriste el inventario");
            panel.SetActive(!panel.activeSelf);
            specialItemsDropdown.gameObject.SetActive(panel.activeSelf);
            useSpecialItemButton.gameObject.SetActive(panel.activeSelf);
        }
    }

    public void CheckForSpecialItem(string itemName)
    {
        if (itemName == "axe" || itemName == "pickaxe" || itemName == "lance")
        {
            if (!specialItems.Contains(itemName))
                specialItems.Add(itemName);

            UpdateDropdown();
            specialItemsDropdown.gameObject.SetActive(true);
            useSpecialItemButton.gameObject.SetActive(true);
        }
    }

    public void UpdateUI(Dictionary<string, int> items)
    {
        contentText.text = "";

        foreach (var item in items)
        {
            contentText.text += $"{item.Key}   {item.Value}\n";
        }
    }

    private void UpdateDropdown()
    {
        specialItemsDropdown.ClearOptions();
        specialItemsDropdown.AddOptions(specialItems);
    }

    private void UseSelectedSpecialItem()
    {
        string selectedItem = specialItemsDropdown.options[specialItemsDropdown.value].text;
        if(selectedItem=="axe")
            Player.Instance.EquipWeapon(0);
        else if (selectedItem == "pickaxe")
            Player.Instance.EquipWeapon(1);
        else if (selectedItem == "lance")
            Player.Instance.EquipWeapon(2);
    }
}
