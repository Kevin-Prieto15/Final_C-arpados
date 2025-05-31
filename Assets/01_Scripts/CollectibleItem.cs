using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Objeto";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(itemName);
            Destroy(gameObject);
        }
    }
}
