using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void NuevoJuego()
    {
        Debug.Log("Boton Nuevo Juego presionado");
        GestorMuerte.ReinicioPorMuerte = false;
        SceneManager.LoadScene("Nivel1");
    }

    public void CargarPartida()
    {
        string escenaGuardada = PlayerPrefs.GetString("UltimaEscena", "Nivel1");
        Debug.Log("Attempting to load scene: " + escenaGuardada);
        SceneManager.LoadScene(escenaGuardada);
    }

    public void GuardarProgresoActual()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("UltimaEscena", escenaActual);
        PlayerPrefs.Save();
    }

    public void Salir()
    {
        Application.Quit();
    }
}