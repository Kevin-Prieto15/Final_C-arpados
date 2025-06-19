using UnityEngine;

public class TrapSpider : MonoBehaviour
{
    public float slowDuration = 5f;     // Tiempo de ralentizaci�n
    public float lifespan = 10f;        // Tiempo de vida de la trampa
    public float slowFactor = 0.5f;     // Qu� tanto se reduce la velocidad
    public int damage = 1;              // Da�o que inflige al jugador

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 1f, 0.25f); // Trampa semitransparente
        }

        Destroy(gameObject, lifespan); // Destruye tras 10 segundos
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);                         // Le quita 1 de vida
                player.ApplySlow(slowFactor, slowDuration);       // Aplica ralentizaci�n
            }

            Destroy(gameObject); // Se destruye al activarse
        }
    }
}
