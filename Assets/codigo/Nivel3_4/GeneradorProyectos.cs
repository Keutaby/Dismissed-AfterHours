
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
            return;
        }

        if (puntosAparicion == null ||
            puntosAparicion.Length < cantidadProyectos)
        {
            return;
        }

        List<Transform> puntosDisponibles =
            new List<Transform>(puntosAparicion);

        
        for (int i = 0; i < puntosDisponibles.Count; i++)
        {
            int aleatorio = Random.Range(i, puntosDisponibles.Count);

            Transform temporal = puntosDisponibles[i];
            puntosDisponibles[i] = puntosDisponibles[aleatorio];
            puntosDisponibles[aleatorio] = temporal;
        }

        //Aquí creamos 5 proyectos :D!!! 
        for (int i = 0; i < cantidadProyectos; i++)
        {
            Transform punto = puntosDisponibles[i];

            Instantiate(
                prefabProyecto,
                punto.position,
                punto.rotation
            );
        }
    }
}
