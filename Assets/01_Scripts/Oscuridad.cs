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

    private float tiempoActual = 0f;
    private enum Estado { Claro, Oscureciendo, Oscuro, Aclarando }
    private Estado estado = Estado.Claro;

    void Start()
    {
        SetAlpha(alphaMin);
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
}
