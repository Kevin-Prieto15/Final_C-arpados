using Assets._01_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Serpiente : MonoBehaviour, IHaveSpawner
{
    [Header("Stats")]
    public float vida = 30f;
    public float da�o = 5f;


    [Header("Movimiento y Animaci�n")]
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

    private enum Estado { Quieto, Persiguiendo, Patrullando }
    private Estado estadoActual = Estado.Quieto;
    private Spawner spawner;
    public GameObject Drop;

    public void SetSpawner(Spawner s)
    {
        spawner = s;
    }
    [Header("Sonidos")]
    public AudioSource afkSound;
    public AudioSource attackSound;
    public AudioSource killSound;

    private bool isDead = false;
    private bool lowHealthActive = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player")?.transform;
        p = player?.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            Debug.LogError("No se encontr� ning�n objeto con tag 'Player'.");

        rb.freezeRotation = true;
        if (afkSound == null || attackSound == null || killSound == null)
        {
            Debug.LogError("Faltan AudioSource asignados en Inspector.");
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(HandleSnakeAudio), 0f, 1f); // Chequea cada segundo

    }

    void Update()
    {
        if (player == null) return;

        cooldownAtaque -= Time.deltaTime;

        float distanciaAlPlayer = Vector2.Distance(transform.position, player.position);

        switch (estadoActual)
        {
            case Estado.Quieto:
                animator.SetFloat("Speed", 0);
                if (distanciaAlPlayer <= distanciaDeteccion && !p.esInvisible)
                {
                    estadoActual = Estado.Persiguiendo;
                }
                break;

            case Estado.Persiguiendo:
                if (p.esInvisible)
                {
                    CambiarAPatrullando(); 
                }
                else if (distanciaAlPlayer >= distanciaAbandono)
                {
                    CambiarAPatrullando();
                }
                else
                {
                    SeguirPlayer();
                }
                break;


            case Estado.Patrullando:
                Patrullar();
                if (distanciaAlPlayer <= distanciaDeteccion && !p.esInvisible)
                {
                    estadoActual = Estado.Persiguiendo;
                }
                break;
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

    public void takeDamage(float da�o)
    {
        vida -= da�o;
        if (vida <= 0)
        {
            spawner.AvisarMuerte();
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
                p.TakeDamage(da�o);
                cooldownAtaque = tiempoEntreAtaques;
            }
           
        }
    }

    void HandleSnakeAudio()
    {
        if (isDead) return;

        if (vida <= 0f)
        {
            killSound.Play();
            isDead = true;
            CancelInvoke(nameof(HandleSnakeAudio));
            return;
        }

        if (vida <= 6f)
        {
            if (!lowHealthActive)
            {
                CancelInvoke(nameof(PlayAfkSound));
                InvokeRepeating(nameof(PlayAttackSound), 0f, 3f);
                lowHealthActive = true;
            }
        }
        else
        {
            if (!afkSound.isPlaying && (estadoActual == Estado.Quieto || estadoActual == Estado.Patrullando))
            {
                InvokeRepeating(nameof(PlayAfkSound), 0f, 5f);
            }
        }
    }

    void PlayAfkSound()
    {
        if (!afkSound.isPlaying)
            afkSound.Play();
    }

    void PlayAttackSound()
    {
        if (!attackSound.isPlaying)
            attackSound.Play();
    }

}
