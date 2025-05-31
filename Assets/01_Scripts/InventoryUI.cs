using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panel;
    public Text contentText;

    private void Start()
    {
       
        panel.SetActive(false);
    }

    void Update()
    {
        Debug.Log("InventoryUI está corriendo"); 

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Abriste el inventario");
            panel.SetActive(!panel.activeSelf);
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
}
