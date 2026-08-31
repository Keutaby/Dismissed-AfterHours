using UnityEngine;
using UnityEngine.InputSystem;

public class FlashMecanica : MonoBehaviour
{
    [Header("Configuración Flash")]
    public float alcanceFlash = 10f;
    public float anguloFlash = 45f;
    public LayerMask capaEnemigo;
    public Light luzTelefono;

    [Header("Referencias")]
    public Transform camaraTransform;

    private bool tieneTelefono = false;
    private bool flashEncendido = false;

    void Start()
    {
        if (camaraTransform == null && Camera.main != null)
            camaraTransform = Camera.main.transform;

        if (luzTelefono != null)
            luzTelefono.enabled = false;
    }

    //when picking up the phone
    public void EquiparTelefono()
    {
        tieneTelefono = true;
        Debug.Log("[FlashlightMechanic] Celular equipado. Flash listo.");
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        // Checks for 'F' key OR Left Mouse Button
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

        // Raycast straight forward from camera view
        Ray rayo = new Ray(camaraTransform.position, camaraTransform.forward);

        if (Physics.Raycast(rayo, out RaycastHit hit, alcanceFlash))
        {
            Enemigo1Tutorial fantasma = hit.collider.GetComponentInParent<Enemigo1Tutorial>();

            if (fantasma != null)
            {
                Debug.Log("[FlashMecanica] ¡Fantasma impactado!");

                // Call stun logic
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