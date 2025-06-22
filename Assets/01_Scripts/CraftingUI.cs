using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public InventoryUI inventoryUI;   // Referencia al inventario
    public CraftingSystem craftingSystem;  // Referencia al sistema de crafteo

    public Button axeCraftButton;
    public Button pickaxeCraftButton;
    public Button lanceCraftButton;

    void Start()
    {
        axeCraftButton.onClick.AddListener(() => Fabricar("axe"));
        pickaxeCraftButton.onClick.AddListener(() => Fabricar("pickaxe"));
        lanceCraftButton.onClick.AddListener(() => Fabricar("lance"));
    }

    void Update()
    {
        // Actualiza botones para activar/desactivar según materiales
        axeCraftButton.interactable = inventoryUI.PuedeFabricar("axe");
        pickaxeCraftButton.interactable = inventoryUI.PuedeFabricar("pickaxe");
        lanceCraftButton.interactable = inventoryUI.PuedeFabricar("lance");
    }

    void Fabricar(string herramienta)
    {
        if (inventoryUI.PuedeFabricar(herramienta))
        {
            inventoryUI.FabricarHerramienta(herramienta);
            Debug.Log(herramienta + " fabricada.");
        }
        else
        {
            Debug.Log("No tienes materiales suficientes para fabricar " + herramienta);
        }
    }
}
