using UnityEngine;
using UnityEngine.SceneManagement;

public class LectorTarjeta : MonoBehaviour
{
    [SerializeField] private string escenaSiguiente;   // nombre exacto de la escena
    [SerializeField] private AudioSource sonidoAcceso;

    private bool usado;

    void OnTriggerEnter(Collider other)
    {
        if (usado) return;
        if (other.GetComponentInParent<TarjetaAcceso>() == null) return;

        usado = true;
        if (sonidoAcceso != null) sonidoAcceso.Play();
        Debug.Log("[Lector] Tarjeta aceptada, cargando " + escenaSiguiente);
        SceneManager.LoadScene(escenaSiguiente);
    }
}