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

    void Start()
    {
        initialPosition = transform.position;

        // Buscar modelos
        modelFront = transform.Find("BuhoFront");
        modelRight = transform.Find("BuhoRight");
        modelLeft = transform.Find("BuhoLeft");

        // Desactivar todos menos Front
        foreach (Transform child in transform)
            child.gameObject.SetActive(child == modelFront);

        // Set animator y sprite del Front al inicio
        animator = modelFront.GetComponent<Animator>();
        sr = modelFront.GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector3 target = (distance <= detectionRadius) ? player.position : initialPosition;

        float distanceToTarget = Vector2.Distance(transform.position, target);

        if (distanceToTarget > stopDistance)
        {
            MoveTowards(target);
        }
        else
        {
            SetModelActive(modelFront);
            animator.SetFloat("DirectionX", 0f);
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        // Activar modelo según dirección
        if (dir.x > 0.1f)
            SetModelActive(modelRight);
        else if (dir.x < -0.1f)
            SetModelActive(modelLeft);
        else
            SetModelActive(modelFront);

        animator.SetFloat("DirectionX", Mathf.Abs(dir.x));

        // Flip visual si va a la izquierda
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
            animator.Play("Right_Fly", 0, 0f); // o "Idle" según el modelo
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
