using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload scenes

public class SistemaSalud : MonoBehaviour
{
    public int saludMaxima = 100;
    private int saludActual;

    public delegate void CambioSalud(float porcentaje);
    public event CambioSalud alCambiarSalud;

    private MonitorMuerte monitorMuerte;

    void Awake()
    {
        saludActual = saludMaxima;
        monitorMuerte = GetComponent<MonitorMuerte>();
    }

    public void RecibirDano(int cantidad)
    {
        saludActual = Mathf.Clamp(saludActual - cantidad, 0, saludMaxima);
        
        float porcentaje = (float)saludActual / saludMaxima;
        alCambiarSalud?.Invoke(porcentaje);

        if (saludActual <= 0)
        {
            // If the object taking fatal damage is the player, restart the level
            if (gameObject.CompareTag("Player") || gameObject.name.Contains("jugador"))
            {
                ReiniciarNivel();
            }
            else
            {
                monitorMuerte?.procesar_muerte();
            }
        }
    }

    public void ReiniciarNivel()
    {
        Debug.Log("[SistemaSalud] El jugador ha muerto. Reiniciando el nivel...");
        // Gets the currently active scene index and reloads it
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual);
    }
}