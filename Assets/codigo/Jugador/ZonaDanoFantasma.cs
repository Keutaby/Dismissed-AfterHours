using UnityEngine;

public class ZonaDanoFantasma : MonoBehaviour
{
    [SerializeField] private int dano = 10;
    [SerializeField] private float intervalo = 1f;

    private float siguienteDano;

    void OnEnable() => siguienteDano = 0f;

    void OnTriggerStay(Collider other)
    {
        if (Time.time < siguienteDano) return;

        var salud = other.GetComponentInParent<SistemaSalud>();
        if (salud == null || !salud.CompareTag("jugador")) return;

        salud.RecibirDano(dano);
        siguienteDano = Time.time + intervalo;
    }
}