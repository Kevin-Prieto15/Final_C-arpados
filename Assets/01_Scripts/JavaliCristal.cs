using Assets._01_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavaliCristal : MonoBehaviour, IHaveSpawner
{
    [Header("Stats")]
    public float vida = 30;
    public int daño = 5;

    [Header("Movimiento y Animación")]
    public float velocidad = 3f;
    public float distanciaDeteccion = 5f;
    public float distanciaAbandono = 8f;
    public float tiempoEntreAtaques = 1f;

    [Header("Patrullaje")]
    public Vector2 tiempoMovimientoAleatorio = new Vector2(2f, 5f);
    public Vector2 tiempoQuietoAleatorio = new Vector2(2f, 5f);

    private Animator animator;
    private Transform player;
    private Player p;
    private Rigidbody2D rb;

    private Vector2 ultimaDireccion = Vector2.down;
    private Vector2 direccionPatrulla = Vector2.zero;

    private float cooldownAtaque = 0f;
    private float temporizadorPatrulla = 0f;
    private bool estaMoviendosePatrulla = false;

    private enum Estado { Quieto, Patrullando, Persiguiendo, TurboRun }
    private Estado estadoActual = Estado.Patrullando;

    private float duracionTurboRun = 2f;
    private float timerTurboRun = 0f;
    private float cooldownTurboRun = 0f;
    private Vector2 direccionTurboRun;
    private Spawner spawner;

    public GameObject Drop;

    public void SetSpawner(Spawner s)
    {
        spawner = s;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player")?.transform;
        p = player?.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            Debug.LogError("No se encontró ningún objeto con tag 'Player'.");

        rb.freezeRotation = true;
        CambiarAPatrullando();
    }

    void Update()
    {
        if (player == null) return;

        cooldownAtaque -= Time.deltaTime;
        cooldownTurboRun -= Time.deltaTime;

        float distanciaAlPlayer = Vector2.Distance(transform.position, player.position);

        switch (estadoActual)
        {
            case Estado.Patrullando:
                Patrullar();

                if (!p.esInvisible && distanciaAlPlayer <= distanciaDeteccion)
                {
                    if (cooldownTurboRun <= 0)
                    {
                        PrepararTurboRun(distanciaAlPlayer);
                    }
                    else
                    {
                        estadoActual = Estado.Persiguiendo; // Aquí lo forzamos a seguirte si el turbo no está listo pero estás cerca
                    }
                }
                break;

            case Estado.Persiguiendo:
                if (distanciaAlPlayer >= distanciaAbandono || p.esInvisible)
                {
                    CambiarAPatrullando();
                }
                else
                {
                    if (cooldownTurboRun <= 0f)
                    {
                        PrepararTurboRun(distanciaAlPlayer);
                    }
                    else
                    {
                        SeguirPlayer();
                    }
                }
                break;

            case Estado.TurboRun:
                EjecutarTurboRun(distanciaAlPlayer);
                break;
        }
    }

    void PrepararTurboRun(float distanciaAlPlayer)
    {
        ultimaDireccion = DireccionSimplificada((player.position - transform.position).normalized);
        direccionTurboRun = ultimaDireccion;
        timerTurboRun = duracionTurboRun;
        estadoActual = Estado.TurboRun;

        animator.speed = 2f;
        animator.SetFloat("DirX", direccionTurboRun.x);
        animator.SetFloat("DirY", direccionTurboRun.y);
        animator.SetFloat("Speed", velocidad * 2);
    }

    void EjecutarTurboRun(float distanciaAlPlayer)
    {
        rb.MovePosition(rb.position + direccionTurboRun * velocidad * 2 * Time.deltaTime);

        timerTurboRun -= Time.deltaTime;

        if (timerTurboRun <= 0f)
        {
            animator.speed = 1f;
            cooldownTurboRun = 10f;

            if (distanciaAlPlayer <= distanciaDeteccion)
            {
                estadoActual = Estado.Persiguiendo;
            }
            else
            {
                CambiarAPatrullando();
            }
        }
    }

    void SeguirPlayer()
    {
        Vector2 direccion = (player.position - transform.position).normalized;
        ultimaDireccion = DireccionSimplificada(direccion);

        animator.SetFloat("DirX", ultimaDireccion.x);
        animator.SetFloat("DirY", ultimaDireccion.y);
        animator.SetFloat("Speed", velocidad);

        rb.MovePosition(rb.position + direccion * velocidad * Time.deltaTime);
    }

    void Patrullar()
    {
        temporizadorPatrulla -= Time.deltaTime;

        if (temporizadorPatrulla <= 0f)
        {
            estaMoviendosePatrulla = !estaMoviendosePatrulla;

            if (estaMoviendosePatrulla)
            {
                direccionPatrulla = DireccionAleatoria();
                ultimaDireccion = direccionPatrulla;

                animator.SetFloat("DirX", ultimaDireccion.x);
                animator.SetFloat("DirY", ultimaDireccion.y);
                animator.SetFloat("Speed", velocidad);

                temporizadorPatrulla = Random.Range(tiempoMovimientoAleatorio.x, tiempoMovimientoAleatorio.y);
            }
            else
            {
                animator.SetFloat("Speed", 0);
                temporizadorPatrulla = Random.Range(tiempoQuietoAleatorio.x, tiempoQuietoAleatorio.y);
            }
        }

        if (estaMoviendosePatrulla)
        {
            rb.MovePosition(rb.position + direccionPatrulla * velocidad * Time.deltaTime);
        }
    }

    Vector2 DireccionAleatoria()
    {
        int dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0: return Vector2.up;
            case 1: return Vector2.down;
            case 2: return Vector2.left;
            case 3: return Vector2.right;
        }
        return Vector2.zero;
    }

    Vector2 DireccionSimplificada(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(dir.x > 0 ? 1 : -1, 0);
        else
            return new Vector2(0, dir.y > 0 ? 1 : -1);
    }

    void CambiarAPatrullando()
    {
        estadoActual = Estado.Patrullando;
        temporizadorPatrulla = 0f;
        estaMoviendosePatrulla = false;
        animator.SetFloat("Speed", 0);
    }

    public void takeDamage(float daño)
    {
        vida -= daño;
        if (vida <= 0)
        {
            spawner.AvisarMuerte();
            Instantiate(Drop, transform.position, Quaternion.identity);
            Instantiate(Drop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && cooldownAtaque <= 0f)
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (!p.esInvisible)
            {
                animator.SetTrigger("IsAttaking");
                p.TakeDamage(daño);
                cooldownAtaque = tiempoEntreAtaques;
            }

        }
    }

}
