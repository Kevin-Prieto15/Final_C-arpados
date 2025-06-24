using UnityEngine;

public class EchoOwl : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject player;
    public GameObject buhoVisual;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Parámetros de combate")]
    public float detectionRange = 6f;
    public float timeToStartAttacking = 1f;
    public float attackCooldown = 1f;

    [Header("Vuelo alrededor del jugador")]
    public float orbitRadius = 3f;
    public float orbitSpeed = 1f;

    [Header("Vista y Transparencia")]
    public GameObject viewFront, viewBack, viewLeft, viewRight;
    public SpriteRenderer[] bodyParts;
    public float transparency = 0.5f;

    private Animator animator;
    private int direction = 1;
    private float timeNearPlayer = 0f;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool playerDetected = false;
    private float orbitAngle = 0f;

   

    [Header("Audio")]
    public AudioSource buhoAudio;

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
            animator.SetBool("IsFlying", false);

        // Ya no es necesario hacer buhoAudio.Play() si usas Play On Awake Att Yo
    }


    void Update()
    {
        if (player == null || animator == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        playerDetected = distance <= detectionRange;

        UpdateDirection();

        if (!isAttacking)
        {
            if (playerDetected)
            {
                timeNearPlayer += Time.deltaTime;
                if (timeNearPlayer >= timeToStartAttacking)
                {
                    isAttacking = true;
                    animator.SetBool("IsFlying", true);

                    //  Reproduce sonido solo una vez al comenzar a volar
                    
                }
            }
            else
            {
                timeNearPlayer = 0f;
            }
        }
        else
        {
            if (!playerDetected)
            {
                isAttacking = false;
                timeNearPlayer = 0f;
                attackTimer = 0f;
                animator.SetBool("IsFlying", false);
                return;
            }

            // Volar directamente sobre el jugador
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


    void FireProjectile()
{
    if (projectilePrefab == null || firePoint == null) return;

    GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    // OwlProjectile owlShot = proj.GetComponent<OwlProjectile>();
    // ya no usamos SetDirection porque ahora se mueve con gravedad
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
