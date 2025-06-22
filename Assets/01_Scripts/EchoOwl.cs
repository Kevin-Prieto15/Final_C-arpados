using UnityEngine;

public class EchoOwl : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject player;
    public GameObject buhoVisual;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Parámetros de combate")]
    public float detectionRange = 3f;
    public float timeToStartAttacking = 1f;
    public float attackCooldown = 1f;

    [Header("Vuelo alrededor del jugador")]
    public float orbitRadius = 3f;
    public float orbitSpeed = 1f;

    [Header("Vista y Transparencia")]
    public GameObject viewFront, viewBack, viewLeft, viewRight;
    public SpriteRenderer[] bodyParts;
    public float transparency = 0.5f;

    [Header("Vuelo aleatorio")]
    public float randomFlyRadius = 2f;
    public float randomFlyInterval = 3f;

    private Animator animator;
    private int direction = 1;
    private float timeNearPlayer = 0f;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool playerDetected = false;
    private float orbitAngle = 0f;

    private Vector2 randomTarget;
    private float randomFlyTimer = 0f;

    void Start()
    {
        if (buhoVisual != null)
            animator = buhoVisual.GetComponent<Animator>();
        else
            Debug.LogError("BuhoVisual no asignado.");

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        SetTransparency(transparency);
        if (animator != null)
            animator.SetBool("IsFlying", true); // Siempre inicia volando
    }

    void Update()
    {
        if (player == null || animator == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        playerDetected = distance <= detectionRange;

        UpdateDirection();

        if (playerDetected)
        {
            timeNearPlayer += Time.deltaTime;

            if (!isAttacking && timeNearPlayer >= timeToStartAttacking)
            {
                isAttacking = true;
            }

            if (isAttacking)
            {
                Vector2 targetPosition = new Vector2(player.transform.position.x, player.transform.position.y + 2.5f);
                transform.position = Vector2.Lerp(transform.position, targetPosition, orbitSpeed * Time.deltaTime);

                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    animator.SetTrigger("Attack");
                    FireProjectile();
                    attackTimer = 0f;
                }
            }
        }
        else
        {
            // Vuelo aleatorio constante cuando no detecta al jugador
            isAttacking = false;
            timeNearPlayer = 0f;
            attackTimer = 0f;

            RandomFlying();
        }
    }

    void RandomFlying()
    {
        // Si el búho está cerca de su destino, elige uno nuevo
        if (Vector2.Distance(transform.position, randomTarget) < 0.5f)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomFlyRadius;
            randomTarget = (Vector2)transform.position + offset;
        }

        // Movimiento más suave y continuo
        float step = orbitSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, randomTarget, step);
    }


    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }

    void UpdateDirection()
    {
        Vector2 dir = player.transform.position - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            direction = dir.x > 0 ? 3 : 2;
        else
            direction = dir.y > 0 ? 0 : 1;

        animator.SetInteger("Direction", direction);

        viewFront.SetActive(direction == 1);
        viewBack.SetActive(direction == 0);
        viewLeft.SetActive(direction == 2);
        viewRight.SetActive(direction == 3);
    }

    void SetTransparency(float alpha)
    {
        foreach (var part in bodyParts)
        {
            if (part != null)
            {
                Color c = part.color;
                c.a = alpha;
                part.color = c;
            }
        }
    }
}
