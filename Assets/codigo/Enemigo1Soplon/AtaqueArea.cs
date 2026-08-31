using UnityEngine;

public class AtaqueArea : MonoBehaviour
{
    public int danoAlJugador = 5;
    public float intervaloDano = 0.5f; // Deals damage every 0.5 seconds
    private float tiempoProximoDano = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("jugador") || other.gameObject.name.Contains("jugador"))
        {
            if (Time.time >= tiempoProximoDano)
            {
                SistemaSalud saludJugador = other.GetComponent<SistemaSalud>();
                if (saludJugador != null)
                {
                    saludJugador.RecibirDano(danoAlJugador);
                    tiempoProximoDano = Time.time + intervaloDano;
                }
            }
        }
    }
}