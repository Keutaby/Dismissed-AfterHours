
using UnityEngine;

public class RecolectarProyecto : MonoBehaviour
{
    private bool recolectado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (recolectado)
            return;

        if (other.CompareTag("jugador") ||
            other.transform.root.CompareTag("jugador"))
        {
            recolectado = true;

            Debug.Log("¡Proyecto recolectado!");

            ContadorProyectos.Instance.AgregarProyecto();

            Destroy(gameObject);
        }
    }
}
