using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsObjetos : MonoBehaviour
{
    [Header("Vida del objeto destructible")]
    public float life = 5f;

    [Header("Prefab que se dropea al morir")]
    public GameObject Drop;
    public GameObject SecondDrop;

    [Header("Prefab de part�culas de corte (cada golpe)")]
    public GameObject particulasChop;

    [Header("Offset personalizado para las part�culas")]
    public Vector3 particulaOffset = Vector3.zero;

    private AudioSource audioSource;
    public float tiempoRespawn = 300f;

    private SpriteRenderer[] sprites;
    private Collider2D[] colliders;
    private bool isDestroyed = false;
    private float tiempoActual = 0f;
    public bool oneDrop=false;

    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        colliders = GetComponentsInChildren<Collider2D>(true);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDestroyed)
        {
            tiempoActual += Time.deltaTime;
            if (tiempoActual >= tiempoRespawn)
            {
                Respawn();
            }
        }
    }

    public void takeDamage(float damage)
    {
        if (isDestroyed) return;

        life -= damage;

        if (oneDrop)
        {
            AudioClip piedraSound = Resources.Load<AudioClip>("Stone");
            if (piedraSound != null)
            {
                AudioSource.PlayClipAtPoint(piedraSound, transform.position);
            }
        }
        else if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        if (particulasChop != null)
        {
            Instantiate(particulasChop, transform.position + particulaOffset, Quaternion.identity);
        }

        if (life <= 0)
        {
            // Dropea objetos
            if (Drop != null) Instantiate(Drop, transform.position, Quaternion.identity);
            if (!oneDrop && Random.Range(1, 4) == 1)
            {
                Instantiate(SecondDrop, new Vector2(transform.position.x + 0.3f, transform.position.y), Quaternion.identity);
            }

            // Desactivar TODOS los sprites y colliders (incluyendo hijos)
            foreach (var s in sprites) s.enabled = false;
            foreach (var c in colliders) c.enabled = false;

            isDestroyed = true;
            tiempoActual = 0f;

            // Sincroniza f�sica por si acaso
            Physics2D.SyncTransforms();
        }
    }

    private void Respawn()
    {
        life = 3f;
        foreach (var s in sprites) s.enabled = true;
        foreach (var c in colliders) c.enabled = true;

        isDestroyed = false;
        tiempoActual = 0f;
    }

}
