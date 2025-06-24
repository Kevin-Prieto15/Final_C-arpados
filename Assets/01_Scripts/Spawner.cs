using Assets._01_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // Los 4 prefabs diferentes
    public Transform[] spawnPoints;    // Los 4 puntos de aparición
    public float tiempoEntreSpawns = 5f;
    public int maxEnemigos = 10;

    [HideInInspector] public int currentEnemies = 0;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tiempoEntreSpawns && currentEnemies < maxEnemigos)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }


    void SpawnEnemy()
    {
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform punto = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemigo = Instantiate(prefab, punto.position, Quaternion.identity);

        currentEnemies++;

        // Le pasa referencia a ESTE Spawner al enemigo
        var spawnerRef = enemigo.GetComponent<IHaveSpawner>();
        if (spawnerRef != null)
        {
            spawnerRef.SetSpawner(this);
        }
    }

    // Método público que pueden llamar los enemigos
    public void AvisarMuerte()
    {
        currentEnemies--;
        if (currentEnemies < 0) currentEnemies = 0; // Por si acaso uwu
    }
}
