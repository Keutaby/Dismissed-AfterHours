using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ReglaT : MonoBehaviour
{
    [Header("Velocidad del golpe (m/s)")]
    [SerializeField] private float velocidadMinima = 1.5f;  // más lento que esto no hace daño
    [SerializeField] private float velocidadMaxima = 6f;    // a esta velocidad hace el daño máximo

    [Header("Daño")]
    [SerializeField] private int danoMinimo = 15;
    [SerializeField] private int danoMaximo = 60;
    [SerializeField] private float cooldownPorFantasma = 0.5f;
    [SerializeField] private float aturdimientoGolpe = 0.4f; // respiro corto tras cada golpe

    private XRGrabInteractable grab;
    private Vector3 ultimaPosicion;
    private float velocidad;
    private readonly Dictionary<SistemaSalud, float> ultimoGolpe = new Dictionary<SistemaSalud, float>();

    void Awake() => grab = GetComponent<XRGrabInteractable>();

    void Start() => ultimaPosicion = transform.position;

    void Update()
    {
        float instantanea = (transform.position - ultimaPosicion).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        velocidad = Mathf.Lerp(velocidad, instantanea, 0.5f); // suaviza picos
        ultimaPosicion = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Regla] Trigger tocó: " + other.name);
        Golpear(other);
    }

    void OnTriggerStay(Collider other) => Golpear(other);

    private void Golpear(Collider col)
    {
        FantasmaSoplon fantasma = col.GetComponentInParent<FantasmaSoplon>();
        if (fantasma == null) return;

        if (!grab.isSelected)
        {
            Debug.Log("[Regla] Toca al fantasma pero NO está agarrada");
            return;
        }

        if (velocidad < velocidadMinima)
        {
            Debug.Log("[Regla] Muy lento: " + velocidad.ToString("F2") + " (mínimo " + velocidadMinima + ")");
            return;
        }

        SistemaSalud salud = fantasma.GetComponent<SistemaSalud>();
        if (salud == null)
        {
            Debug.LogWarning("[Regla] El fantasma no tiene SistemaSalud en el mismo objeto que FantasmaSoplon");
            return;
        }
        if (salud.EstaMuerto) return;

        if (ultimoGolpe.TryGetValue(salud, out float t) && Time.time < t + cooldownPorFantasma) return;
        ultimoGolpe[salud] = Time.time;

        float fuerza = Mathf.InverseLerp(velocidadMinima, velocidadMaxima, velocidad);
        float danoBase = Mathf.Lerp(danoMinimo, danoMaximo, fuerza);
        int dano = Mathf.RoundToInt(danoBase * fantasma.MultiplicadorDanoRegla);

        fantasma.Aturdir(aturdimientoGolpe);   // antes del daño, por si lo mata
        salud.RecibirDano(dano);
        Debug.Log("[Regla] Velocidad " + velocidad.ToString("F1") + " → daño " + dano + " | vida: " + salud.SaludActual);
    }
}