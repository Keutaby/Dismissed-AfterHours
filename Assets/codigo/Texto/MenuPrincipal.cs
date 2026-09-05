using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void NuevoJuego()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        Application.Quit();
    }
}