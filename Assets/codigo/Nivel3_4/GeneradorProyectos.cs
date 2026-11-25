
using System.Collections.Generic;
using UnityEngine;

public class GeneradorProyectos : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabProyecto;
    public Transform[] puntosAparicion;
    public int cantidadProyectos = 5;

    public void GenerarProyectos()
    {
        if (prefabProyecto == null)
        {
            Debug.LogError("Falta asignar el prefab del proyecto.");
            return;
        }

        if (puntosAparicion == null ||
            puntosAparicion.Length < cantidadProyectos)
        {
            Debug.LogError("No hay suficientes puntos de aparición.");
            return;
        }

        List<Transform> puntosDisponibles =
            new List<Transform>(puntosAparicion);

        // Mezclar los puntos aleatoriamente
        for (int i = 0; i < puntosDisponibles.Count; i++)
        {
            int aleatorio = Random.Range(i, puntosDisponibles.Count);

            Transform temporal = puntosDisponibles[i];
            puntosDisponibles[i] = puntosDisponibles[aleatorio];
            puntosDisponibles[aleatorio] = temporal;
        }

        // Crear los cinco proyectos
        for (int i = 0; i < cantidadProyectos; i++)
        {
            Transform punto = puntosDisponibles[i];

            Instantiate(
                prefabProyecto,
                punto.position,
                punto.rotation
            );
        }

        Debug.Log("Se generaron " + cantidadProyectos + " proyectos.");
    }
}
