using UnityEngine;

public class OwlSound : MonoBehaviour
{
    private AudioSource audioSource;
    public float intervalo = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = Resources.Load<AudioClip>("BUHO");
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = 1f;

        InvokeRepeating(nameof(ReproducirSonido), 0f, intervalo);
    }

    void ReproducirSonido()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
