using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class FlashMecanica : MonoBehaviour
{
    public Light luzTelefono;
    public float alcanceFlash = 10f;
    public LayerMask capaEnemigo;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.activated.AddListener(AlternarFlash);
        grabInteractable.selectExited.AddListener(OnSoltar);
    }

    void OnDisable()
    {
        grabInteractable.activated.RemoveListener(AlternarFlash);
        grabInteractable.selectExited.RemoveListener(OnSoltar);
    }

    private void AlternarFlash(ActivateEventArgs args)
    {
        Debug.Log("[FlashMecanica] Presionado en el teléfono!");

        if (luzTelefono != null)
        {
            luzTelefono.enabled = !luzTelefono.enabled;
            Debug.Log("[FlashMecanica] Estado del flash: " + luzTelefono.enabled);
        }
        else
        {
            Debug.LogWarning("[FlashMecanica] No hay ninguna luz asignado en luzTelefono.");
        }

        if (luzTelefono != null && luzTelefono.enabled)
        {
            EjecutarRayoFlash();
        }
    }

    private void EjecutarRayoFlash()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, alcanceFlash, capaEnemigo))
        {
            Debug.Log("[FlashMecanica] Rayo impacto en: " + hit.collider.gameObject.name);

            Enemigo1Tutorial fantasma = hit.collider.GetComponentInParent<Enemigo1Tutorial>();
            if (fantasma != null)
            {
                fantasma.DeslumbradoPorFlash();

                SistemaSalud saludGhost = fantasma.GetComponent<SistemaSalud>();
                if (saludGhost != null)
                {
                    saludGhost.RecibirDano(20);
                }
            }
        }
    }

    private void OnSoltar(SelectExitEventArgs args)
    {
        if (luzTelefono != null)
        {
            luzTelefono.enabled = false;
        }
    }
}