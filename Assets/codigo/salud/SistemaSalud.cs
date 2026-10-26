using System;
using UnityEngine;

public class SistemaSalud : MonoBehaviour
{
    [SerializeField] private int saludMaxima = 100;

    public int SaludActual { get; private set; }
    public float Porcentaje => (float)SaludActual / saludMaxima;
    public bool EstaMuerto => SaludActual <= 0;

    public event Action<float> alCambiarSalud;
    public event Action alMorir;

    private MonitorMuerte monitorMuerte; // temporal, para que el fantasma actual siga compilando

    void Awake()
    {
        SaludActual = saludMaxima;
        monitorMuerte = GetComponent<MonitorMuerte>();
    }

    public void RecibirDano(int cantidad)
    {
        if (EstaMuerto) return;

        SaludActual = Mathf.Clamp(SaludActual - cantidad, 0, saludMaxima);
        alCambiarSalud?.Invoke(Porcentaje);

        if (EstaMuerto)
        {
            alMorir?.Invoke();
            monitorMuerte?.procesar_muerte();
        }
    }

    [ContextMenu("Probar daño (10)")]
    private void ProbarDano() => RecibirDano(10);
}