using UnityEngine;

public class Jabali : MonoBehaviour
{
    [Header("Parámetros del Jabali")]
    public float detectionRange = 4f;
    public float attackRange = 1.2f;
    public float speed = 2f;
    public Transform player;             // Referencia al jugador
    public Transform spriteObject;       // Objeto que contiene el Animator

    private Animator animator;           // Referencia al Animator
    private Rigidbody2D rb;              // Rigidbody2D del jabalí
    private string currentAnimation = "idle";
    private Vector2 startPosition;       // Posición inicial para retornar
    private Player playerScript;         // Referencia al script del jugador

    void Start()
    {
        // Validar spriteObject asignado
        if (spriteObject == null)
        {
            Debug.LogError("spriteObject no asignado en " + gameObject.name);
            enabled = false;
            return;
        }

        // Obtener el Animator del sprite
        animator = spriteObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No se encontró Animator en spriteObject: " + spriteObject.name);
            enabled = false;
            return;
        }

        // Obtener el Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No se encontró Rigidbody2D en " + gameObject.name);
            enabled = false;
            return;
        }

        // Guardar posición inicial
        startPosition = rb.position;

        // Obtener el script del jugador para verificar invisibilidad
        playerScript = player.GetComponent<Player>();
        if (playerScript == null)
        {
            Debug.LogError("No se encontró el script Player en el objeto asignado");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(rb.position, player.position);

        // Si el jugador es visible y está dentro del rango → perseguir
        if (distance < detectionRange && !playerScript.EstaInvisible())
        {
            ChasePlayer(distance);
        }
        else
        {
            // Si es invisible o está fuera de rango → regresar al origen
            ReturnToOrigin();
        }
    }


    void ChasePlayer(float distance)
    {
        // Movimiento hacia el jugador
        Vector2 newPosition = Vector2.MoveTowards(rb.position, player.position, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Decidir animación de movimiento o ataque según distancia y dirección
        bool isLeftOfPlayer = rb.position.x > player.position.x;
        currentAnimation = distance < attackRange ?
            (isLeftOfPlayer ? "atackLeft" : "atackRight") :
            (isLeftOfPlayer ? "walkLeft" : "walkRight");

        PlayAnimation(currentAnimation);
    }

    void ReturnToOrigin()
    {
        // Movimiento hacia la posición inicial
        Vector2 newPosition = Vector2.MoveTowards(rb.position, startPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Animar retorno si no ha llegado
        if (Vector2.Distance(rb.position, startPosition) > 0.1f)
        {
            bool isLeftOfStart = rb.position.x > startPosition.x;
            string anim = isLeftOfStart ? "walkLeft" : "walkRight";
            PlayAnimation(anim);
        }
        else
        {
            PlayAnimation("idle");
        }
    }

    void PlayAnimation(string animationName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            animator.Play(animationName);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el radio de detección en la escena
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
