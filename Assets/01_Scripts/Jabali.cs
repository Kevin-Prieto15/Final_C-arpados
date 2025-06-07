using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jabali : MonoBehaviour
{
    public float detectionRange = 4f;
    public float attackRange = 1.2f;
    public float speed = 2f;
    public Transform player;
    public Transform spriteObject; // ← arrastra el hijo con el Animator

    private Animator animator;
    private Vector2 randomDirection;
    private float changeDirectionTime = 3f;
    private float timer;

    void Start()
    {
        animator = spriteObject.GetComponent<Animator>();
        ChooseRandomDirection();
        timer = changeDirectionTime;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            bool isLeftOfPlayer = transform.position.x < player.position.x;

            if (distance < attackRange)
            {
                animator.Play(isLeftOfPlayer ? "atackRight" : "atackLeft");
            }
            else
            {
                animator.Play(isLeftOfPlayer ? "walkRight" : "walkLeft");
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChooseRandomDirection();
                timer = changeDirectionTime;
            }

            transform.Translate(randomDirection * speed * 0.5f * Time.deltaTime);

            // Solo reproducir animación de caminar según dirección
            if (Mathf.Abs(randomDirection.x) > 0.01f)
            {
                animator.Play(randomDirection.x > 0 ? "walkRight" : "walkLeft");
            }
            else
            {
                // Si no hay movimiento horizontal, seguir reproduciendo última animación de caminar
                animator.Play("walkRight");
            }
        }
    }

    void ChooseRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        randomDirection = new Vector2(x, y).normalized;
    }
}
