using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsObjetos : MonoBehaviour
{
    [Header("Vida del objeto destructible")]
    public float life = 5f;

    [Header("Prefab que se dropea al morir")]
    public GameObject Drop;

    [Header("Prefab de partículas de corte (cada golpe)")]
    public GameObject particulasChop;

    [Header("Offset personalizado para las partículas")]
    public Vector3 particulaOffset = Vector3.zero;

    private AudioSource audioSource;

    public void takeDamage(float damage)
    {
        if (gameObject.name.ToLower().Contains("piedra") || gameObject.tag == "Piedra")
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

        life -= damage;

        if (life <= 0f)
        {
            if (Drop != null)
            {
                Instantiate(Drop, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

}
