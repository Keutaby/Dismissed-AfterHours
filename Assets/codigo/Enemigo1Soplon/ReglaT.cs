using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class AtaqueRegla : MonoBehaviour
{
    public int danoRegla = 25;
    public float velocidadMinimaImpacto = 1.5f;
    public LayerMask capaEnemigo;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        grabInteractable.activated.AddListener(OnTriggerPressed);
    }

    void OnDisable()
    {
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    private void OnTriggerPressed(ActivateEventArgs args)
    {
        EjecutarAtaqueVR();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!grabInteractable.isSelected) return;
        if (collision.relativeVelocity.magnitude >= velocidadMinimaImpacto)
        {
            ProcesarDano(collision.gameObject);
        }
    }

    private void EjecutarAtaqueVR()
    {
        Debug.Log("[AtaqueRegla] Ataque activado");
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.5f, capaEnemigo))
        {
            ProcesarDano(hit.collider.gameObject);
        }
    }

    private void ProcesarDano(GameObject objetivo)
    {
        SistemaSalud salud = objetivo.GetComponentInParent<SistemaSalud>();
        if (salud != null)
        {
            salud.RecibirDano(danoRegla);
            Debug.Log("[AtaqueRegla] Dano infligido al enemigo!");
        }
    }
}