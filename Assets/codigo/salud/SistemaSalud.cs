using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (gameObject.CompareTag("jugador") || gameObject.name.Contains("jugador"))
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
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual);
    }
}