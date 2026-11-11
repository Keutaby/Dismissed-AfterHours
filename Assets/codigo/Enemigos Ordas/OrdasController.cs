using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class Orda
{
    public string nombreOrda;              // Nombre de la orda
    public GameObject enemyPrefab;         // El prefab del enemigo para esta orda
    public int enemyCount;                 // Cantidad de enemigos que aparecen
    public float spawnInterval;            // Segundos entre la aparición de cada enemigo
}

public class OrdasController : MonoBehaviour
{
    [Header("Configuración de Oleadas")]
    public List<Orda> ordas;
    public Transform[] spawnPoints;        // Puntos donde pueden aparecer (deben estar sobre la NavMesh)

    [Header("Tiempos")]
    public float timeBetweenWaves = 5f;    // Descanso entre ordas
    [SerializeField] private bool iniciarAlEmpezar = true;   // Desmárcalo si quieres lanzarlas desde otro script

    [Header("Eventos")]
    [SerializeField] private UnityEvent alIniciarOrda;       // por ejemplo: sonido de alarma
    [SerializeField] private UnityEvent alCompletarTodas;    // por ejemplo: abrir la puerta de salida

    private int currentWaveIndex;
    private int enemiesAlive;
    private bool iniciado;

    void Start()
    {
        if (spawnPoints.Length == 0)
            Debug.LogWarning("[Ordas] No hay Spawn Points asignados, aparecerán en la posición de este objeto");

        if (iniciarAlEmpezar) IniciarOrdas();
    }

    // Llámalo desde TutorialPaneles (Al Terminar), un trigger, un botón, etc.
    public void IniciarOrdas()
    {
        if (iniciado || ordas.Count == 0) return;
        iniciado = true;
        StartCoroutine(RutinaOrdas());
    }

    private IEnumerator RutinaOrdas()
    {
        while (currentWaveIndex < ordas.Count)
        {
            Orda orda = ordas[currentWaveIndex];
            Debug.Log("[Ordas] ¡Iniciando " + orda.nombreOrda + "!");
            alIniciarOrda?.Invoke();

            enemiesAlive = orda.enemyCount;

            for (int i = 0; i < orda.enemyCount; i++)
            {
                if (!SpawnEnemy(orda.enemyPrefab)) enemiesAlive--;   // si falla el spawn, no esperes por él
                yield return new WaitForSeconds(orda.spawnInterval);
            }

            // Espera a que mueran todos
            while (enemiesAlive > 0) yield return null;

            Debug.Log("[Ordas] " + orda.nombreOrda + " completada");
            currentWaveIndex++;

            if (currentWaveIndex < ordas.Count)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("[Ordas] ¡Todas las ordas completadas!");
        alCompletarTodas?.Invoke();
    }

    private bool SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return false;

        Transform punto = spawnPoints.Length > 0 ? spawnPoints[Random.Range(0, spawnPoints.Length)] : transform;

        // Ajusta el punto a la NavMesh, si no el NavMeshAgent da error
        Vector3 posicion = punto.position;
        if (NavMesh.SamplePosition(punto.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            posicion = hit.position;
        else
            Debug.LogWarning("[Ordas] El punto " + punto.name + " no está cerca de la NavMesh");

        GameObject enemigo = Instantiate(prefab, posicion, punto.rotation);

        SistemaSalud salud = enemigo.GetComponent<SistemaSalud>();
        if (salud == null)
        {
            Debug.LogError("[Ordas] El prefab " + prefab.name + " no tiene SistemaSalud");
            Destroy(enemigo);
            return false;
        }

        salud.alMorir += () => AlMorirEnemigo(enemigo);
        return true;
    }

    private void AlMorirEnemigo(GameObject enemigo)
    {
        enemiesAlive--;
        Debug.Log("[Ordas] Enemigos restantes: " + enemiesAlive);
        if (enemigo != null) Destroy(enemigo, 1f);   // limpia al fantasma muerto
    }
}