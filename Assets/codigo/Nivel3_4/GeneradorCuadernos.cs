using UnityEngine;
using System.Collections.Generic;

public class GeneradorCuadernos : MonoBehaviour
{
    [Header("Configuración cuadernos")]
    public GameObject CuadernoTarea;
    public Transform[] puntosAparicion;
    public int cantidadCuadernos = 3;

    public void GenerarCuadernos()
    {
        if(CuadernoTarea == null)
        {
            Debug.LogError("FaltaAsignar el cuaderno, como vas a hacer la tarea???");
            return;
        }

        if(puntosAparicion.Length < cantidadCuadernos)
        {
            Debug.LogError("No hay suficientes puntos de aparicion");
            return;
        }

        List<Transform> puntosDisponibles = new List<Transform>(puntosAparicion);
        for( int i = 0; i < puntosDisponibles.Count; i++)
        {
            int aleatorio = Random.Range(i, puntosDisponibles.Count);

            Transform temporal = puntosDisponibles[i];
            puntosDisponibles[i] = puntosDisponibles[aleatorio];
            puntosDisponibles[aleatorio] = temporal;
        }

        for(int i = 0; i < cantidadCuadernos; i++)
        {
            Transform punto = puntosDisponibles[i];
            
            Instantiate(
                CuadernoTarea,
                punto.position,
                punto.localRotation
            );
        }

        Debug.Log("Se generaron" + cantidadCuadernos + "cuadernos");
    }
}
