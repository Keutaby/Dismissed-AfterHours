using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Orda
{
    public string nombreOrda;                // Nombre de la orda
    public GameObject enemyPrefab;         // El prefab del enemigo para esta orda
    public int enemyCount;                 // Cantidad de enemigos que aperecen
    public float spawnInterval;            // Tiempo de espera entre la aparición de cada enemigo
}

public class OrdasController : MonoBehaviour
{
    [Header("Configuración de Oleadas")]
    public List<Orda> ordas;               // Lista de todas las ordas configuradas
    public Transform[] spawnPoints;        // Puntos donde pueden aparecer los enemigos

    [Header("Tiempos")]
    public float timeBetweenWaves = 5f;    // Tiempo de descanso entre orfass

    private int currentWaveIndex = 0;
    private bool canSpawn = true;
    private int enemiesAlive = 0;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("¡No hay puntos de aparición (Spawn Points) asignados!");
        }

        // Inicia la primera oleada si existe
        if (ordas.Count > 0)
        {
            StartCoroutine(SpawnWave());
        }
    }

    void Update()
    {
        
    }

    IEnumerator SpawnWave()
    {
        canSpawn = false;
        Orda ordaActual = ordas[currentWaveIndex];

        Debug.Log("¡Iniciando " + ordaActual.nombreOrda + "!");

        // Establecemos cuántos enemigos habrá vivos en esta orda
        enemiesAlive = ordaActual.enemyCount;

        // Bucle para generar a los enemigos de la orda actual
        for (int i = 0; i < ordaActual.enemyCount; i++)
        {
            SpawnEnemy(ordaActual.enemyPrefab);

            // Espera el tiempo configurado antes de generar al siguiente enemigo
            yield return new WaitForSeconds(ordaActual.spawnInterval);
        }

        // Esperar a que todos los enemigos mueran antes de pasar a la siguiente oleada
       
        while (enemiesAlive > 0)
        {
            yield return null;
        }

        // Oleada completada
        Debug.Log(ordaActual.nombreOrda + " completada.");
        currentWaveIndex++;

        // Verificar si quedan más oleadas
        if (currentWaveIndex < ordas.Count)
        {
            Debug.Log("Siguiente oleada en " + timeBetweenWaves + " segundos...");
            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(SpawnWave());
        }
        else
        {
            Debug.Log("¡Todas las oleadas han sido completadas! ¡Victoria!");
            // Aquí puedes activar tu pantalla de victoria, etc.
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return;

        // Elegir un punto de aparición aleatorio (si hay varios) o usar la posición del Spawner si no hay puntos
        Transform spawnPoint = transform;
        if (spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        // Instanciar el enemigo
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

       
    }

    // Método que se llama cuando un enemigo muere
    void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}
