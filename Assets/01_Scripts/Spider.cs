using UnityEngine;
using System.Collections;
using Assets._01_Scripts;

public class Spider : MonoBehaviour, IHaveSpawner
{
    [Header("Referencias")]
    public GameObject trapPrefab;
    public GameObject player;


    [Header("Efectos")]
    public GameObject dustEffect;


    [Header("Movimiento y combate")]
    public float moveSpeed = 2f;
    public float detectionRange = 3f;
    public float biteDamage = 10f;
    public float biteCooldown = 2f;
    public float vida = 50;

    [Header("Trampas")]
    public float trapInterval = 5f;
    public float trapSpawnRadius = 3f;

    [Header("Patrullaje")]
    public float patrolChangeInterval = 2f;

    private Animator animator;
    private Vector2 patrolDirection;
    private float patrolTimer;
    private bool isEngagingPlayer = false;
    private Coroutine currentActionCoroutine = null;

    private bool canBite = true;
    private Spawner spawner;
    public GameObject drop;

    public void SetSpawner(Spawner s)
    {
        spawner = s;
    }
    private float dustCooldown = 0.1f; // o 0.05f si quieres m�s part�culas
    private float dustTimer = 0f;


    public AudioClip Spider_Sound;
    public AudioClip Spider_Walk;

    
    [Header("Sonidos")]
    public AudioSource hitAudio;
    public AudioSource walkAudio;

    private float walkTimer = 0f;
    private float walkInterval = 3f;


    [Header("Vida")]
    public float maxHealth = 3f;
    private float currentHealth;
    private bool isDead = false;




    void Start()
    {
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj;
        }

        patrolTimer = patrolChangeInterval;
        patrolDirection = Vector2.down;

        Patrol();

        // Cargar los dos AudioSources correctamente
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource src in sources)
        {
            if (src.clip == Spider_Sound) hitAudio = src;
            else if (src.clip == Spider_Walk) walkAudio = src;
        }

        currentHealth = maxHealth;
    }



    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange && !Player.Instance.esInvisible)
            DecideAction();
        else
        {
            isEngagingPlayer = false;
            Patrol();
        }

        if (animator.GetBool("IsWalking"))
            SpawnDust();

        // Reproduce Spider_Walk cada 3 segundos, si no se est� reproduciendo ya
        walkTimer += Time.deltaTime;
        if (walkTimer >= walkInterval)
        {
            if (walkAudio != null && !walkAudio.isPlaying)
            {
                walkAudio.Play();
            }
            walkTimer = 0f;
        }
    }



    void Patrol()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            patrolDirection = Random.insideUnitCircle.normalized;
            patrolTimer = patrolChangeInterval;
        }

        UpdateDirection(patrolDirection);
        animator.SetBool("IsWalking", true);

        transform.position += (Vector3)(patrolDirection * moveSpeed * Time.deltaTime);
        SpawnDust();

    }

    void DecideAction()
    {
        if (isEngagingPlayer) return;

        isEngagingPlayer = true;

        if (currentActionCoroutine != null)
        {
            StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = null;
        }

        bool willUseTraps = Random.value < 0.5f;

        if (willUseTraps)
            currentActionCoroutine = StartCoroutine(TrapSpamMode());
        else
            currentActionCoroutine = StartCoroutine(FollowPlayer());
    }

    IEnumerator TrapSpamMode()
    {
        float duration = 5f;
        float timer = 0f;

        while (timer < duration)
        {
            Vector2 dir = (transform.position - player.transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            SpawnDust();


            UpdateDirection(player.transform.position - transform.position);

            animator.SetBool("IsWalking", true);
            animator.SetTrigger("Attack");

            SpawnTrap();

            yield return new WaitForSeconds(1.5f);
            timer += 1.5f;
        }

        animator.SetBool("IsWalking", false);
        isEngagingPlayer = false;
        currentActionCoroutine = null;
    }

    IEnumerator FollowPlayer()
    {
        float duration = 3f;
        float timer = 0f;

        while (timer < duration)
        {
            Vector2 dir = (player.transform.position - transform.position).normalized;
            UpdateDirection(dir);
            animator.SetBool("IsWalking", true);
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("IsWalking", false);
        isEngagingPlayer = false;
        currentActionCoroutine = null;
    }

    void SpawnTrap()
    {
        Vector2 offset = Random.insideUnitCircle * trapSpawnRadius;
        Instantiate(trapPrefab, player.transform.position + (Vector3)offset, Quaternion.identity);
    }

    void UpdateDirection(Vector2 dir)
    {
        int direction = 1;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            direction = dir.x > 0 ? 3 : 2;
        else
            direction = dir.y > 0 ? 0 : 1;

        animator.SetInteger("Direction", direction);

        Transform viewFront = transform.Find("View_Front");
        Transform viewBack = transform.Find("View_Back");
        Transform viewLeft = transform.Find("View_Left");
        Transform viewRight = transform.Find("View_Right");

        if (viewFront != null) viewFront.gameObject.SetActive(direction == 1);
        if (viewBack != null) viewBack.gameObject.SetActive(direction == 0);
        if (viewLeft != null) viewLeft.gameObject.SetActive(direction == 2);
        if (viewRight != null) viewRight.gameObject.SetActive(direction == 3);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!canBite) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (!p.esInvisible)
            {
                animator.SetTrigger("IsAttaking");
                p.TakeDamage(biteDamage);
                StartCoroutine(BiteCooldown());
            }

        }
    }
    public void takeDamage(float da�o)
    {
        vida -= da�o;
        if (vida <= 0)
        {
            spawner.AvisarMuerte();
            Instantiate(drop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private IEnumerator BiteCooldown()
    {
        canBite = false;
        yield return new WaitForSeconds(biteCooldown);
        canBite = true;
    }

    void SpawnDust()
    {
        if (dustEffect == null) return;

        dustTimer -= Time.deltaTime;

        if (dustTimer <= 0f)
        {
            Vector3 offset = new Vector3(0, -0.4f, 0);
            GameObject dust = Instantiate(dustEffect, transform.position + offset, Quaternion.identity);
            Destroy(dust, 1f); // destruye despu�s de 1 segundo
            dustTimer = dustCooldown;
        }
    }







    public void TakeDamageFromPlayer(float damage)
    {
        if (isDead) return;

        if (hitAudio != null && !hitAudio.isPlaying)
            hitAudio.Play();

        currentHealth -= damage;
        Debug.Log($"Ara�a recibi� {damage} de da�o. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log(" La ara�a ha muerto.");

        // Opcional: destruir o desactivar
        Destroy(gameObject, 1.5f); // espera 1.5s para dejar ver animaci�n
    }


}