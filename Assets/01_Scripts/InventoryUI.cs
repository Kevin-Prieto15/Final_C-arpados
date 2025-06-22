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

    public Button createToolButton; // Asignar en Inspector
    public Text createToolButtonText; // Texto para mostrar "CREAR HERRAMIENTA"

    public CraftingSystem craftingSystem;
    public CraftingPanel craftingPanel;  // Asigna desde el Inspector


    private void Start()
    {
        // Ocultar elementos UI al inicio
        specialItemsDropdown.gameObject.SetActive(false);
        useSpecialItemButton.gameObject.SetActive(false);

        if (createToolButton != null)
        {
            createToolButton.gameObject.SetActive(false);
            // Asignar el método para abrir panel de crafteo cuando se clickea el botón
            createToolButton.onClick.RemoveAllListeners();
            createToolButton.onClick.AddListener(AbrirPanelCrafteo);
        }

        useSpecialItemButton.onClick.AddListener(UseSelectedSpecialItem);

        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf)
            {
                UpdateUI(InventoryManager.Instance.GetInventory());
                UpdateSpecialItemsDropdown();
                ActualizarVisibilidadHerramientas();
            }
            else
            {
                specialItemsDropdown.gameObject.SetActive(false);
                useSpecialItemButton.gameObject.SetActive(false);
                if (createToolButton != null)
                    createToolButton.gameObject.SetActive(false);
            }
        }
    }

    public void AbrirPanelCrafteo()
    {
        if (craftingPanel != null)
        {
            panel.SetActive(false); // OCULTA el panel de inventario
            craftingPanel.MostrarPanel(); // MUESTRA el panel de crafteo
        }
        else
        {
            Debug.LogWarning("CraftingPanel no asignado en InventoryUI");
        }
    }



    public void UpdateUI(Dictionary<string, int> items)
    {
        contentText.text = "";

        specialItems.Clear();

        foreach (var item in items)
        {
            contentText.text += $"{item.Key}   {item.Value}\n";

            // Chequea si es herramienta para agregar a dropdown
            if (item.Key == "axe" || item.Key == "pickaxe" || item.Key == "lance")
            {
                if (!specialItems.Contains(item.Key))
                    specialItems.Add(item.Key);
            }
        }

        UpdateSpecialItemsDropdown();
    }

    private void UpdateSpecialItemsDropdown()
    {
        if (specialItems.Count > 0)
        {
            specialItemsDropdown.gameObject.SetActive(true);
            useSpecialItemButton.gameObject.SetActive(true);

            specialItemsDropdown.ClearOptions();
            specialItemsDropdown.AddOptions(specialItems);
        }
        else
        {
            specialItemsDropdown.gameObject.SetActive(false);
            useSpecialItemButton.gameObject.SetActive(false);
        }
    }

    private void UseSelectedSpecialItem()
    {
        string selectedItem = specialItemsDropdown.options[specialItemsDropdown.value].text;
        if (selectedItem == "axe")
            Player.Instance.EquipWeapon(0);
        else if (selectedItem == "pickaxe")
            Player.Instance.EquipWeapon(1);
        else if (selectedItem == "lance")
            Player.Instance.EquipWeapon(2);
    }

    public bool PuedeFabricar(string herramienta)
    {
        if (craftingSystem == null || InventoryManager.Instance == null)
            return false;

        return craftingSystem.TieneMateriales(InventoryManager.Instance.GetInventory(), herramienta);
    }

    public void FabricarHerramienta(string herramienta)
    {
        if (!PuedeFabricar(herramienta)) return;

        var inventarioActual = InventoryManager.Instance.GetInventory();
        craftingSystem.RestarMateriales(inventarioActual, herramienta);

        if (inventarioActual.ContainsKey(herramienta))
            inventarioActual[herramienta]++;
        else
            inventarioActual[herramienta] = 1;

        InventoryManager.Instance.UpdateInventoryUI();
    }

    private void ActualizarVisibilidadHerramientas()
    {
        bool hayHerramientas = specialItems.Count > 0;
        specialItemsDropdown.gameObject.SetActive(hayHerramientas);
        useSpecialItemButton.gameObject.SetActive(hayHerramientas);

        if (createToolButton != null)
        {
            createToolButton.gameObject.SetActive(true); // Siempre visible

            bool puedeCrear = false;
            string textoFaltante = "";

            var inventario = InventoryManager.Instance.GetInventory();

            foreach (var herramienta in craftingSystem.recetas.Keys)
            {
                if (craftingSystem.TieneMateriales(inventario, herramienta))
                {
                    puedeCrear = true;
                    break;
                }
                else if (string.IsNullOrEmpty(textoFaltante))
                {
                    textoFaltante = "Faltan: " + ObtenerTextoFaltantes(herramienta);
                }
            }

        }
    }
    private string ObtenerTextoFaltantes(string herramienta)
    {
        if (!craftingSystem.recetas.ContainsKey(herramienta)) return "";

        var receta = craftingSystem.recetas[herramienta];
        var inventario = InventoryManager.Instance.GetInventory();

        List<string> faltan = new List<string>();

        foreach (var mat in receta)
        {
            int cantidad = inventario.ContainsKey(mat.Key) ? inventario[mat.Key] : 0;
            int diferencia = mat.Value - cantidad;
            if (diferencia > 0)
                faltan.Add($"{diferencia} {mat.Key}");
        }

        return string.Join(", ", faltan);
    }


    private bool PuedeCrearAlgunaHerramienta()
    {
        if (craftingSystem == null || InventoryManager.Instance == null) return false;

        var inventario = InventoryManager.Instance.GetInventory();

        foreach (var herramienta in craftingSystem.recetas.Keys)
        {
            if (craftingSystem.TieneMateriales(inventario, herramienta))
                return true;
        }
        return false;
    }
}
