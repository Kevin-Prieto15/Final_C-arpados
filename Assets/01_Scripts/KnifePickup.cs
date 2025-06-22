using UnityEngine;

public class KnifePickup : MonoBehaviour
{
    public string nombre = "Cuchillo";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player jugador = other.GetComponent<Player>();
            if (jugador != null)
            {
                jugador.tieneArma = true;
                jugador.textoDebug.text = "Recogiste un cuchillo";
            }

            InventoryManager inventario = FindObjectOfType<InventoryManager>();
            if (inventario != null)
            {
                inventario.AddItem(nombre);
            }

            Destroy(gameObject);
        }
    }
}
