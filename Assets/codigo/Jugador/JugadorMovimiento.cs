using UnityEngine;
using UnityEngine.InputSystem;

public enum EstadosMovimiento {
    quieto,
    caminando,
    saltando
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class JugadorMovimiento : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidad_movimiento = 0.5f;
    public float velocidad_rotacion = 1.0f;

    [Header("Interacción y UI")]
    public float distanciaInteraccion = 2.5f;
    public Transform manoJugador;
    public GameObject mapaUI;
    private bool mapaActivo = false;

    public delegate void CambioEstadoEvento(EstadosMovimiento estadoNuevo);
    public event CambioEstadoEvento hayGenteEscuchandoElEstado;

    private EstadosMovimiento estadoActual = EstadosMovimiento.quieto;
    private bool isCrouching = false;

    private Rigidbody rigidBody;
    private PlayerInput entradasDelJugador;
    private InputAction movimiento;
    private InputAction saltar;
    private Transform indicacionDireccion;

    void Start()
    {
        entradasDelJugador = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody>();

        movimiento = entradasDelJugador.actions.FindAction("movimiento");
        saltar = entradasDelJugador.actions.FindAction("saltar");

        if (saltar != null)
        {
            saltar.performed += SaltaJugadorSalta;
        }

        if (Camera.main != null)
        {
            indicacionDireccion = Camera.main.transform;
        }
    }

    void OnDestroy()
    {
        if (saltar != null)
        {
            saltar.performed -= SaltaJugadorSalta;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Press [E] to interact with items
        if (keyboard.eKey.wasPressedThisFrame)
        {
            Interactuar();
        }

        // Press [O] to toggle Map UI
        if (keyboard.oKey.wasPressedThisFrame && mapaUI != null)
        {
            mapaActivo = !mapaActivo;
            mapaUI.SetActive(mapaActivo);
        }

        // Press [C] to Crouch
        if (keyboard.cKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
        }
    }

    void Interactuar()
    {
        if (indicacionDireccion == null) return;

        //cast a Ray from the Main Camera straight forward
        Ray rayo = new Ray(indicacionDireccion.position, indicacionDireccion.forward);

        if (Physics.Raycast(rayo, out RaycastHit hit, distanciaInteraccion))
        {
            //check InteractuableComportamiento component
            InteractuableComportamiento objetoInteractuable = hit.collider.GetComponent<InteractuableComportamiento>();

            if (objetoInteractuable != null)
            {
                Transform puntoDeAgarre = (manoJugador != null) ? manoJugador : transform;
                objetoInteractuable.colocar_en(puntoDeAgarre);
            }
        }
    }

    void FixedUpdate()
    {
        if (movimiento == null) return;

        Vector2 direccion = movimiento.ReadValue<Vector2>();

        if (direccion.magnitude > 0.1f)
        {
            CambiarEstado(EstadosMovimiento.caminando);
            Avanzar(direccion);
        }
        else
        {
            CambiarEstado(EstadosMovimiento.quieto);
        }
    }

    void SaltaJugadorSalta(InputAction.CallbackContext context)
    {
        bool estamosTocandoSuelo = false;
        Ray rayoHaciaElSuelo = new Ray(transform.position, Vector3.down * 0.6f);

        if (Physics.Raycast(rayoHaciaElSuelo, out RaycastHit chocamosCon, 0.8f))
        {
            if (chocamosCon.collider.CompareTag("floor") || chocamosCon.collider.CompareTag("Untagged"))
            {
                estamosTocandoSuelo = true;
            }
        }

        if (estamosTocandoSuelo)
        {
            rigidBody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            CambiarEstado(EstadosMovimiento.saltando);
        }
    }

    void Avanzar(Vector2 direccionJoystick)
    {
        if (indicacionDireccion == null) return;

        Vector3 adelante = indicacionDireccion.forward;
        Vector3 derecha = indicacionDireccion.right;

        adelante.y = 0f;
        derecha.y = 0f;

        Vector3 haciaAdelante = (adelante * direccionJoystick.y + derecha * direccionJoystick.x).normalized;

        float multiplicadorVelocidad = isCrouching ? 0.5f : 1.0f;
        Vector3 desplazamiento = haciaAdelante * (velocidad_movimiento * multiplicadorVelocidad) * Time.fixedDeltaTime;

        rigidBody.MovePosition(rigidBody.position + desplazamiento);
    }

    void Mirar(Vector3 direccionMovimiento)
    {
        if (direccionMovimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, rotacionObjetivo, velocidad_rotacion * Time.fixedDeltaTime));
        }
    }

    void CambiarEstado(EstadosMovimiento estadoNuevo)
    {
        if (estadoActual == estadoNuevo) return;

        estadoActual = estadoNuevo;
        hayGenteEscuchandoElEstado?.Invoke(estadoNuevo);
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
}