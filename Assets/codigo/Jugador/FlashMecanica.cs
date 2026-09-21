using UnityEngine;
using UnityEngine.InputSystem;

public class FlashMecanica : MonoBehaviour
{
    public float alcanceFlash = 10f;
    public float anguloFlash = 45f;
    public LayerMask capaEnemigo;
    public Light luzTelefono;
    public Transform camaraTransform;
    private bool tieneTelefono = false;
    private bool flashEncendido = false;

    void Start()
    {
        if (camaraTransform == null && Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
        }

        if (luzTelefono != null)
        {
            luzTelefono.enabled = false;
        }
    }

    public void EquiparTelefono()
    {
        tieneTelefono = true;

        Debug.Log("[FlashlightMechanic] Celular equipado. Flash listo.");
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        bool fPressed = keyboard != null && keyboard.fKey.wasPressedThisFrame;
        bool leftClickPressed = mouse != null && mouse.leftButton.wasPressedThisFrame;

        if (fPressed || leftClickPressed)
        {
            UsarFlash();
        }
    }

    public void UsarFlash()
    {
        Debug.Log("[FlashMecanica] Flash activado!");
        // Toggles the Spotlight visual on and off
        if (luzTelefono != null)
        {
            luzTelefono.enabled = !luzTelefono.enabled;
        }

        if (camaraTransform == null) return;

        Ray rayo = new Ray(camaraTransform.position, camaraTransform.forward);

        if (Physics.Raycast(rayo, out RaycastHit hit, alcanceFlash))
        {
            Enemigo1Tutorial fantasma = hit.collider.GetComponentInParent<Enemigo1Tutorial>();

            if (fantasma != null)
            {
                Debug.Log("[FlashMecanica] ¡Fantasma impactado!");
                fantasma.DeslumbradoPorFlash();
                
                // Apply damage to health bar
                SistemaSalud saludGhost = fantasma.GetComponent<SistemaSalud>();
                if (saludGhost != null)
                {
                    saludGhost.RecibirDano(20); // Drains 20 HP per flash
                }
            }
        }
    }
}

