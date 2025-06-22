using UnityEngine;
using UnityEngine.UI;
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

        btnFabricarHacha.interactable = inventoryUI.PuedeFabricar("axe");
        btnFabricarPico.interactable = inventoryUI.PuedeFabricar("pickaxe");
        btnFabricarLanza.interactable = inventoryUI.PuedeFabricar("lance");
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
