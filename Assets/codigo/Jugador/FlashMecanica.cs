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
        if (luzTelefono != null)
        {
            luzTelefono.enabled = !luzTelefono.enabled;
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