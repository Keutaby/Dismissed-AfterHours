
using UnityEngine;
using UnityEngine.InputSystem;

public class PruebaGolpeRector : MonoBehaviour
{
    [Header("Referencias")]
    public AtaqueManos ataqueManos;
    public VidaDirector vidaDirector;

    [Header("Configuracion")]
    public int golpesNecesarios = 3;

    private int golpesActuales = 0;

    void Update()
    {
        // Detectar si presionamos K
        if (Keyboard.current != null &&
            Keyboard.current.kKey.wasPressedThisFrame)
        {
            SimularGolpe();
        }
    }

    public void SimularGolpe()
    {
        if (ataqueManos == null || vidaDirector == null)
        {
            Debug.LogError("Faltan referencias en PruebaGolpeRector.");
            return;
        }

        if (!ataqueManos.EstaVulnerable)
        {
            Debug.Log("El Rector todavía no es vulnerable.");
            return;
        }

        golpesActuales++;

        Debug.Log(
            "¡Golpe de prueba! " +
            golpesActuales + "/" + golpesNecesarios
        );

        if (golpesActuales >= golpesNecesarios)
        {
            vidaDirector.RecibirDanio();

            golpesActuales = 0;

            Debug.Log("¡Mano del Rector destruida mediante prueba!");
        }
    }
}
