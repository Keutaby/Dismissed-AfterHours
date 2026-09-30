using System.Collections;
using UnityEngine;

public class CambiarMaterialRepetidamente : MonoBehaviour
{
    private Renderer objetoRenderer;

    // Lista de materiales que irán rotando
    public Material[] listaMateriales;

    // Tiempo de espera entre cada cambio (en segundos)
    public float intervaloTiempo = 1f;

    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();

        // Iniciamos la corrutina
        StartCoroutine(RutinaCambioMaterial());
    }

    IEnumerator RutinaCambioMaterial()
    {
        int indice = 0;

        // Bucle infinito (o puedes usar una condición si quieres que termine)
        while (true)
        {
            if (listaMateriales.Length > 0)
            {
                // Asignamos el material actual del array
                objetoRenderer.material = listaMateriales[indice];

                // Pasamos al siguiente material de la lista, volviendo al inicio si llega al final
                indice = (indice + 1) % listaMateriales.Length;
            }

            // Esperamos el tiempo especificado antes del próximo cambio
            yield return new WaitForSeconds(intervaloTiempo);
        }
    }
}