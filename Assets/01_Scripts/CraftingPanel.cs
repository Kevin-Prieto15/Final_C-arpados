using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Si usas TextMeshPro para textos

public class CraftingPanel : MonoBehaviour
{
    public InventoryUI inventoryUI; // Referencia al InventoryUI
    public TextMeshProUGUI feedbackText; // Texto para mostrar mensajes (opcional)

    [Header("Botones de fabricar")]
    public Button btnFabricarHacha;
    public Button btnFabricarPico;
    public Button btnFabricarLanza;

    private void Start()
    {
        btnFabricarHacha.onClick.AddListener(() => Fabricar("axe"));
        btnFabricarPico.onClick.AddListener(() => Fabricar("pickaxe"));
        btnFabricarLanza.onClick.AddListener(() => Fabricar("lance"));

        // Inicialmente ocultar panel y feedback
        gameObject.SetActive(false);
        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            ActualizarBotones();
        }
    }

    public void MostrarPanel()
    {
        gameObject.SetActive(true);
        ActualizarBotones();
        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void OcultarPanel()
    {
        gameObject.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";

        if (inventoryUI != null)
            inventoryUI.panel.SetActive(true); // muestra el inventario al cerrar crafteo
    }

    void ActualizarBotones()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI no asignado en CraftingPanel");
            return;
        }

        ActualizarBotonIndividual(btnFabricarHacha, "axe");
        ActualizarBotonIndividual(btnFabricarPico, "pickaxe");
        ActualizarBotonIndividual(btnFabricarLanza, "lance");
    }

    void ActualizarBotonIndividual(Button boton, string herramienta)
    {
        bool puede = inventoryUI.PuedeFabricar(herramienta);
        boton.interactable = puede;

        var textoTMP = boton.GetComponentInChildren<TextMeshProUGUI>();
        var textoUI = boton.GetComponentInChildren<Text>();

        string nombreVisible = herramienta.ToUpper(); // HACHA, PICKAXE...

        string textoFinal = puede
            ? $"FABRICAR {nombreVisible}"
            : $"{nombreVisible}: Falta: {ObtenerTextoFaltante(herramienta)}";

        if (textoTMP != null)
        {
            textoTMP.text = textoFinal;
        }
        else if (textoUI != null)
        {
            textoUI.text = textoFinal;
        }
        else
        {
            Debug.LogWarning($"No se encontró componente de texto en el botón de {herramienta}");
        }
    }

    string ObtenerTextoFaltante(string herramienta)
    {
        if (inventoryUI == null || inventoryUI.craftingSystem == null) return "";

        var receta = inventoryUI.craftingSystem.recetas[herramienta];
        var inventario = InventoryManager.Instance.GetInventory();

        List<string> faltan = new List<string>();

        foreach (var mat in receta)
        {
            int cantidad = inventario.ContainsKey(mat.Key) ? inventario[mat.Key] : 0;
            int diferencia = mat.Value - cantidad;
            if (diferencia > 0)
            {
                faltan.Add($"{diferencia} {mat.Key}");
            }
        }

        return string.Join(", ", faltan);
    }

    public void Fabricar(string herramienta)
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI no asignado en CraftingPanel");
            return;
        }

        if (inventoryUI.PuedeFabricar(herramienta))
        {
            inventoryUI.FabricarHerramienta(herramienta);
            MostrarFeedback($"¡Has fabricado un {herramienta}!");
        }
        else
        {
            MostrarFeedback($"No tienes materiales suficientes para fabricar un {herramienta}.");
        }
    }

    void MostrarFeedback(string mensaje)
    {
        if (feedbackText != null)
        {
            feedbackText.text = mensaje;
            CancelInvoke(nameof(BorrarFeedback));
            Invoke(nameof(BorrarFeedback), 3f);
        }
    }

    void BorrarFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
