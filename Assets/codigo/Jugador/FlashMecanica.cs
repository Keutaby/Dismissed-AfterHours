using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashMecanica : MonoBehaviour
{
    [Header("Luz")]
    [SerializeField] private Light luzTelefono;

    [Header("Cono del flash")]
    [SerializeField] private float alcanceFlash = 8f;
    [SerializeField] private float anguloCono = 40f;       // ángulo total del cono
    [SerializeField] private LayerMask capaEnemigo;        // la capa donde pusiste a los fantasmas
    [SerializeField] private LayerMask capasObstaculos;    // paredes (Nothing si no quieres que bloqueen)

    [Header("Efecto")]
    [SerializeField] private int dano = 20;
    [SerializeField] private float duracionAturdimiento = 3f;
    [SerializeField] private float cooldown = 1f;

    private XRGrabInteractable grab;
    private float siguienteUso;
    private readonly HashSet<FantasmaSoplon> yaGolpeados = new HashSet<FantasmaSoplon>();

    void Awake() => grab = GetComponent<XRGrabInteractable>();

    void OnEnable()
    {
        grab.activated.AddListener(AlternarFlash);
        grab.selectExited.AddListener(OnSoltar);
    }

    void OnDisable()
    {
        grab.activated.RemoveListener(AlternarFlash);
        grab.selectExited.RemoveListener(OnSoltar);
    }
    private void OnSoltar(SelectExitEventArgs args)
    {
        if (luzTelefono != null) luzTelefono.enabled = false;
    }
    private void AlternarFlash(ActivateEventArgs args)
    {
        if (luzTelefono == null)
        {
            Debug.LogError("[Flash] Luz Telefono NO está asignada en el Inspector");
            return;
        }

        luzTelefono.enabled = !luzTelefono.enabled;
        Debug.Log("[Flash] Luz encendida: " + luzTelefono.enabled);

        if (luzTelefono.enabled && Time.time >= siguienteUso)
        {
            siguienteUso = Time.time + cooldown;
            DeslumbrarFantasmas();
        }
    }

    private void DeslumbrarFantasmas()
    {
        Vector3 origen = luzTelefono.transform.position;
        Vector3 frente = luzTelefono.transform.forward;

        yaGolpeados.Clear();
        Collider[] cercanos = Physics.OverlapSphere(origen, alcanceFlash, capaEnemigo, QueryTriggerInteraction.Ignore);
        Debug.Log("[Flash] Colliders en rango y capa: " + cercanos.Length);

        foreach (Collider col in cercanos)
        {
            FantasmaSoplon fantasma = col.GetComponentInParent<FantasmaSoplon>();
            if (fantasma == null)
            {
                Debug.Log("[Flash] " + col.name + " no tiene FantasmaSoplon en sus padres");
                continue;
            }
            if (!yaGolpeados.Add(fantasma)) continue;

            Vector3 haciaFantasma = col.bounds.center - origen;
            float angulo = Vector3.Angle(frente, haciaFantasma);
            Debug.Log("[Flash] Ángulo al fantasma: " + angulo + " (máximo " + anguloCono * 0.5f + ")");

            if (angulo > anguloCono * 0.5f) continue;
            if (Physics.Linecast(origen, col.bounds.center, capasObstaculos)) continue;
        

            fantasma.Aturdir(duracionAturdimiento * fantasma.MultiplicadorAturdimiento);

            SistemaSalud salud = fantasma.GetComponent<SistemaSalud>();
            if (salud != null) salud.RecibirDano(Mathf.RoundToInt(dano * fantasma.MultiplicadorDanoFlash));
        }
    }

    // Dibuja el cono en la Scene view con el celular seleccionado
    private void OnDrawGizmosSelected()
    {
        if (luzTelefono == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(luzTelefono.transform.position, luzTelefono.transform.forward * alcanceFlash);
        Gizmos.DrawWireSphere(luzTelefono.transform.position, alcanceFlash);
    }
}