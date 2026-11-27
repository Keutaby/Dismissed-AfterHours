using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Solo para el nivel del rector. Va en la regla, en lugar de ReglaT.
[RequireComponent(typeof(XRGrabInteractable))]
public class ReglaTRector : MonoBehaviour
{
    [Header("Golpe")]
    [SerializeField] private float velocidadMinima = 1f;     // más lento que esto no cuenta
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private int golpesPorVentana = 1;       // manos que se pierden en cada ventana de vulnerabilidad

    [Header("Referencias (opcional, si se dejan vacías se buscan solas)")]
    [SerializeField] private VidaDirector vida;

    private XRGrabInteractable grab;
    private Vector3 ultimaPosicion;
    private float velocidad;
    private float siguienteGolpe;
    private readonly Dictionary<AtaqueManos, int> golpesEnVentana = new Dictionary<AtaqueManos, int>();

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (vida == null) vida = FindFirstObjectByType<VidaDirector>();
    }

    void Start() => ultimaPosicion = transform.position;

    void Update()
    {
        float instantanea = (transform.position - ultimaPosicion).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        velocidad = Mathf.Lerp(velocidad, instantanea, 0.5f);
        ultimaPosicion = transform.position;

        // Cuando una mano deja de estar vulnerable, se reinicia su conteo
        foreach (AtaqueManos m in new List<AtaqueManos>(golpesEnVentana.Keys))
            if (m == null || !m.EstaVulnerable) golpesEnVentana[m] = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Regla] Trigger tocó: " + other.name);
        Golpear(other);
    }

    void OnTriggerStay(Collider other) => Golpear(other);

    private void Golpear(Collider col)
    {
        AtaqueManos mano = col.GetComponentInParent<AtaqueManos>();
        if (mano == null) return;

        if (!grab.isSelected || grab.firstInteractorSelecting is XRSocketInteractor)
        {
            Debug.Log("[Regla] Toca la mano pero NO está en la mano del jugador");
            return;
        }

        if (velocidad < velocidadMinima)
        {
            Debug.Log("[Regla] Muy lento: " + velocidad.ToString("F2") + " (mínimo " + velocidadMinima + ")");
            return;
        }

        if (!mano.EstaVulnerable)
        {
            Debug.Log("[Regla] La mano todavía no está vulnerable");
            return;
        }

        if (Time.time < siguienteGolpe) return;

        golpesEnVentana.TryGetValue(mano, out int golpes);
        if (golpes >= golpesPorVentana) return;

        if (vida == null)
        {
            Debug.LogWarning("[Regla] No encontré el VidaDirector en la escena");
            return;
        }

        siguienteGolpe = Time.time + cooldown;
        golpesEnVentana[mano] = golpes + 1;
        vida.RecibirDanio();
        Debug.Log("[Regla] ¡Golpe a la mano del rector!");
    }
}