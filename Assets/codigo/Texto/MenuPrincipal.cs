using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void NuevoJuego()
    {
        Debug.Log("Boton Nuevo Juego presionado");
        GestorMuerte.ReinicioPorMuerte = false;
        SceneManager.LoadScene("Nivel1");
    }

    public void Salir()
    {
        Application.Quit();
    }
}