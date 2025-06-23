using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNIght : MonoBehaviour
{
    public GameObject mapaDiaGO;
    public GameObject mapaNocheGO;

    public float intervaloMinutos = 10f;
    private float tiempo = 0f;
    private bool esNoche = false;

    void Start()
    {
        mapaDiaGO.SetActive(true);
        mapaNocheGO.SetActive(false);
    }

    void Update()
    {
        tiempo += Time.deltaTime / 60f;

        if (tiempo >= intervaloMinutos)
        {
            esNoche = !esNoche;
            mapaDiaGO.SetActive(!esNoche);
            mapaNocheGO.SetActive(esNoche);
            tiempo = 0f;
        }
    }
}

