using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OwlProjectile : MonoBehaviour
{
    public float gravityScale = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        Destroy(gameObject, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger con: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Tocó al jugador con trigger!");
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(5f);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Debug.Log("Tocó el suelo");
            Destroy(gameObject);
        }
    }
}
