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
    [SerializeField] private float aturdimientoGolpe = 0.4f; // un respiro corto tras cada golpe

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

    void OnCollisionEnter(Collision c) => Golpear(c.collider);
    void OnCollisionStay(Collision c) => Golpear(c.collider);

    private void Golpear(Collider col)
    {
        if (!grab.isSelected) return;                 // solo cuando la traes en la mano
        if (velocidad < velocidadMinima) return;

        FantasmaSoplon fantasma = col.GetComponentInParent<FantasmaSoplon>();
        if (fantasma == null) return;

        SistemaSalud salud = fantasma.GetComponent<SistemaSalud>();
        if (salud == null || salud.EstaMuerto) return;

        if (ultimoGolpe.TryGetValue(salud, out float t) && Time.time < t + cooldownPorFantasma) return;
        ultimoGolpe[salud] = Time.time;

        float fuerza = Mathf.InverseLerp(velocidadMinima, velocidadMaxima, velocidad);
        int dano = Mathf.RoundToInt(Mathf.Lerp(danoMinimo, danoMaximo, fuerza));

        fantasma.Aturdir(aturdimientoGolpe);          // antes del daño, por si lo mata
        salud.RecibirDano(dano);
        Debug.Log("[Regla] Velocidad " + velocidad.ToString("F1") + " → daño " + dano + " | vida: " + salud.SaludActual);
    }
}