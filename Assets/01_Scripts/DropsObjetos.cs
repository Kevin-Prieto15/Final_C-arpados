using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsObjetos : MonoBehaviour
{
    public float life = 3f;
    public GameObject Drop;
    public GameObject SecondDrop;

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

            // Sincroniza física por si acaso
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
