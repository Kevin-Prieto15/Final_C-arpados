using UnityEngine;
using System.Collections;
using Assets._01_Scripts;

public class Spider : MonoBehaviour, IHaveSpawner
{
    [Header("Referencias")]
    public GameObject trapPrefab;
    public GameObject player;

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
    public void takeDamage(float daño)
    {
        vida -= daño;
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
}
