using UnityEngine;

public class Buho : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float detectionRadius = 3f;
    public float stopDistance = 0.3f;

    private Vector3 initialPosition;

    private Transform modelFront;
    private Transform modelRight;
    private Transform modelLeft;

    private Animator animator;
    private SpriteRenderer sr;

    private AudioSource audioSource;
    private bool isFlying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuración forzada del AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = 1f;

        // Carga manual del audio si no está asignado
        if (audioSource.clip == null)
        {
            audioSource.clip = Resources.Load<AudioClip>("BUHO"); // Asegúrate de que esté en Assets/Resources/BUHO.wav
        }

        initialPosition = transform.position;

        modelFront = transform.Find("BuhoFront");
        modelRight = transform.Find("BuhoRight");
        modelLeft = transform.Find("BuhoLeft");

        foreach (Transform child in transform)
            child.gameObject.SetActive(child == modelFront);

        animator = modelFront.GetComponent<Animator>();
        sr = modelFront.GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector3 target = (distance <= detectionRadius) ? player.position : initialPosition;

        float distanceToTarget = Vector2.Distance(transform.position, target);
        bool estaAtacando = (distance <= detectionRadius && distanceToTarget > stopDistance);

        if (estaAtacando)
        {
            if (!isFlying)
            {
                isFlying = true;

                if (audioSource != null && audioSource.clip != null)
                {
                    Debug.Log("Reproduciendo sonido del búho");
                    audioSource.Play();
                }
            }

            MoveTowards(target);
        }
        else
        {
            if (isFlying)
                isFlying = false;

            SetModelActive(modelFront);
            animator.SetFloat("DirectionX", 0f);
        }

        // TEST: reproducir con tecla P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (audioSource != null && audioSource.clip != null)
            {
                Debug.Log("Sonido forzado manualmente.");
                audioSource.Play();
            }
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        if (dir.x > 0.1f)
            SetModelActive(modelRight);
        else if (dir.x < -0.1f)
            SetModelActive(modelLeft);
        else
            SetModelActive(modelFront);

        animator.SetFloat("DirectionX", Mathf.Abs(dir.x));

        if (sr != null)
            sr.flipX = dir.x < 0f;
    }

    void SetModelActive(Transform model)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(child == model);

        animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Play("Right_Fly", 0, 0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
