using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Oscuridad : MonoBehaviour
{
    public SpriteRenderer spriteOscuro;

    [Header("Tiempos (segundos)")]
    public float tiempoEnLuz = 300f;       // 5 minutos
    public float tiempoDeTransicion = 60f; // 1 minuto
    public float tiempoEnOscuridad = 300f; // 5 minutos

    [Header("Opacidad")]
    [Range(0f, 1f)] public float alphaMin = 0f;
    [Range(0f, 1f)] public float alphaMax = 230f / 255f;

    [Header("Sonidos de ambiente")]
    public AudioClip sonidoDia;
    public AudioClip sonidoNoche;
    public AudioSource audioSource;

    private float tiempoActual = 0f;
    private enum Estado { Claro, Oscureciendo, Oscuro, Aclarando }
    private Estado estado = Estado.Claro;

    void Start()
    {
        SetAlpha(alphaMin);
        audioSource.clip = sonidoDia;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        tiempoActual += Time.deltaTime;

        switch (estado)
        {
            case Estado.Claro:
                if (tiempoActual >= tiempoEnLuz)
                {
                    tiempoActual = 0f;
                    estado = Estado.Oscureciendo;
                }
                break;

            case Estado.Oscureciendo:
                SetAlpha(Mathf.Lerp(alphaMin, alphaMax, tiempoActual / tiempoDeTransicion));
                if (tiempoActual >= tiempoDeTransicion)
                {
                    tiempoActual = 0f;
                    estado = Estado.Oscuro;
                    CambiarSonido(sonidoNoche);
                }
                break;

            case Estado.Oscuro:
                if (tiempoActual >= tiempoEnOscuridad)
                {
                    tiempoActual = 0f;
                    estado = Estado.Aclarando;
                }
                break;

            case Estado.Aclarando:
                SetAlpha(Mathf.Lerp(alphaMax, alphaMin, tiempoActual / tiempoDeTransicion));
                if (tiempoActual >= tiempoDeTransicion)
                {
                    tiempoActual = 0f;
                    estado = Estado.Claro;
                    CambiarSonido(sonidoDia);
                }
                break;
        }
    }

    private void SetAlpha(float a)
    {
        Color color = spriteOscuro.color;
        color.a = a;
        spriteOscuro.color = color;
    }

    private void CambiarSonido(AudioClip nuevoClip)
    {
        if (audioSource.clip != nuevoClip)
        {
            audioSource.clip = nuevoClip;
            audioSource.Play();
        }
    }

}
