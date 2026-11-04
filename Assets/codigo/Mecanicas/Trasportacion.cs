using UnityEngine;
using UnityEngine.SceneManagement; // necesario para cambiar de escena

public class CambioDeEscenaTarjeta : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de la escena a la que quieres viajar")]
    public string nombreEscenaDestino;

    [Header("Filtro de Objeto")]
    [Tooltip("El Tag del objeto que debe entrar a la zona para activar el cambio")]
    public string tagRequerido = "credencial";

 
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que entró tiene el tag correcto
        if (other.CompareTag(tagRequerido))
        {
            CambiarEscena();
        }
    }

       // nos envia a la otra escena
    void CambiarEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscenaDestino))
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.LogWarning("No se ha asignado a donde papito");
        }
    }
}