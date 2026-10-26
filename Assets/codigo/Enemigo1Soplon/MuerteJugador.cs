using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SistemaSalud))]
public class MuerteJugador : MonoBehaviour
{
    void OnEnable() => GetComponent<SistemaSalud>().alMorir += Reiniciar;
    void OnDisable() => GetComponent<SistemaSalud>().alMorir -= Reiniciar;

    private void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}