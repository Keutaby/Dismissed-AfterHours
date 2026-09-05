using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;



public class Enemigo2 : MonoBehaviour, MonitorMuerte

{

    public enum EstadoEnemigo
    {

        vigilancia,

        sospecha,

        ataque,

        persecucion,

        huida,

        impactado,

        grito,

        desaparece,

        neutraliza

    }



    public EstadoEnemigo estadoActual
    {

        get => _estadoActual;

        set
        {

            _estadoActual = value;

            alCambiarEstado?.Invoke(_estadoActual);

        }

    }



    [SerializeField]

    private EstadoEnemigo _estadoActual = EstadoEnemigo.vigilancia;



    public delegate void CambioEstadoHandler(EstadoEnemigo nuevoEstado);

    public event CambioEstadoHandler alCambiarEstado;



    public Transform transformaJugador;

    private NavMeshAgent agent;

    private GameObject[] punto;



    public float rangoVision = 8f;

    public float distanciaLejos = 15f;

    public float distanciaAtaque = 4f;

    public LayerMask capaCobertura;



    public GameObject zonaAtaqueVisual;

    private bool ejecutandoAtaque = false;



    void Start()

    {

        agent = GetComponent<NavMeshAgent>();

        punto = GameObject.FindGameObjectsWithTag("punto");



        if (transformaJugador == null)
        {

            try
            {

                GameObject jugadorObj = GameObject.FindGameObjectWithTag("jugador");

                if (jugadorObj != null)
                {

                    transformaJugador = jugadorObj.transform;

                }

            }

            catch
            {

                // no tag found yet

            }

        }

    }



    public void IniciarModoAtaque()

    {

        if (!ejecutandoAtaque && estadoActual == EstadoEnemigo.ataque)

        {

            StartCoroutine(RutinaAtaqueArea());

        }

    }



    void Update()

    {

        switch (estadoActual)

        {

            case EstadoEnemigo.vigilancia:

                ModoVigilancia();

                break;



            case EstadoEnemigo.sospecha:

                ModoSospecha();

                break;



            case EstadoEnemigo.ataque:

                ModoAtaque();

                break;



            case EstadoEnemigo.persecucion:

                ModoPersecucion();

                break;



            case EstadoEnemigo.huida:

                ModoHuida();

                break;



            case EstadoEnemigo.impactado:

                ModoImpactado();

                break;



            case EstadoEnemigo.grito:

                ModoGrito();

                break;



            case EstadoEnemigo.desaparece:

                ModoDesaparece();

                break;



            case EstadoEnemigo.neutraliza:

                ModoNeutraliza();

                break;



            default:

                break;

        }

    }



    void ModoVigilancia()
    {

        MoverseEntrePuntos();



        if (jugadorCubierto())
        {

            CambiarEstado(EstadoEnemigo.sospecha);

        }

        else if (VerJugador())
        {

            CambiarEstado(EstadoEnemigo.ataque);

        }

    }



    void ModoSospecha()
    {

        if (VerJugador())
        {

            CambiarEstado(EstadoEnemigo.ataque);

        }

        else if (jugadorLejos())
        {

            CambiarEstado(EstadoEnemigo.vigilancia);

        }

    }



    void ModoAtaque()
    {

        if (agent != null && transformaJugador != null)

        {

            agent.isStopped = false;

            agent.SetDestination(transformaJugador.position);

        }



        // Continuously checks if it can ignite a new attack circle

        IniciarModoAtaque();



        if (jugadorEscapa())

        {

            CambiarEstado(EstadoEnemigo.persecucion);

        }

    }



    void ModoPersecucion()
    {

        if (agent != null && transformaJugador != null)
        {

            agent.isStopped = false;

            agent.SetDestination(transformaJugador.position);

        }



        if (jugadorEscapa())
        {

            CambiarEstado(EstadoEnemigo.vigilancia);

        }

        else if (EnemigoGana())
        {

            CambiarEstado(EstadoEnemigo.ataque);

        }

    }



    void ModoHuida()
    {

        if (EnemigoVencido())
        {

            CambiarEstado(EstadoEnemigo.desaparece);

        }

    }



    void ModoImpactado()
    {

        if (agent != null)
        {

            agent.isStopped = true;

        }



        if (jugadorEscapa())
        {

            CambiarEstado(EstadoEnemigo.vigilancia);

        }



        if (jugadorEnZona())
        {

            CambiarEstado(EstadoEnemigo.grito);

        }

        else
        {

            CambiarEstado(EstadoEnemigo.vigilancia);

        }

    }



    void ModoGrito()
    {

        AlertaAGuardias();



        if (jugadorEscapa())
        {

            CambiarEstado(EstadoEnemigo.vigilancia);

        }

    }



    void ModoDesaparece()
    {

        RestablecerTiempo();

        Destroy(gameObject);

    }



    void ModoNeutraliza()
    {

        MoviemientoJugadorDesabilitado();

    }



    void CambiarEstado(EstadoEnemigo nuevoEstado)

    {

        estadoActual = nuevoEstado;



        if (agent != null)

        {

            agent.isStopped = false;

        }



        // Trigger the attack circle ONCE when entering attack mode

        if (nuevoEstado == EstadoEnemigo.ataque)

        {

            IniciarModoAtaque();

        }

        else

        {

            RestablecerTiempo();

        }

    }



    public void DeslumbradoPorFlash()
    {

        if (estadoActual == EstadoEnemigo.ataque)
        {

            CambiarEstado(EstadoEnemigo.impactado);

        }



        if (agent != null)

        {

            agent.isStopped = true;

        }

    }



    private void RestablecerTiempo()
    {

        Time.timeScale = 1.0f;

        Time.fixedDeltaTime = 0.02f;

    }



    private void MoverseEntrePuntos()
    {

        if (punto != null && punto.Length > 0 && agent != null)
        {

            agent.isStopped = false;

            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {

                int puntoAleatorio = Random.Range(0, punto.Length);

                agent.SetDestination(punto[puntoAleatorio].transform.position);

            }

        }

    }



    bool jugadorCubierto()
    {

        if (transformaJugador == null)
        {

            return false;

        }



        Vector3 direccion = (transformaJugador.position + Vector3.up * 1f) - transform.position;

        float distancia = Vector3.Distance(transform.position, transformaJugador.position);



        if (Physics.Raycast(transform.position, direccion, out RaycastHit hit, distancia, capaCobertura))
        {

            if (hit.transform != transformaJugador)
            {

                return true;

            }

        }

        return false;

    }



    bool VerJugador()
    {

        if (transformaJugador == null)
        {

            return false;

        }



        float distancia = Vector3.Distance(transform.position, transformaJugador.position);

        if (distancia <= rangoVision && !jugadorCubierto())
        {

            return true;

        }

        return false;

    }



    bool jugadorLejos()
    {

        if (transformaJugador == null)
        {

            return true;

        }

        return Vector3.Distance(transform.position, transformaJugador.position) >= distanciaLejos;

    }



    bool jugadorEscapa()
    {

        return jugadorLejos() || jugadorCubierto();

    }



    bool jugadorEnZona()
    {

        if (transformaJugador == null)
        {

            return false;

        }

        return Vector3.Distance(transform.position, transformaJugador.position) <= rangoVision;

    }



    private IEnumerator RutinaAtaqueArea()

    {

        ejecutandoAtaque = true;



        // 1. Show attack circle visual & activate trigger

        if (zonaAtaqueVisual != null)

        {

            zonaAtaqueVisual.SetActive(true);

        }



        // 2. Keep attack active for 3 seconds

        yield return new WaitForSeconds(3.0f);



        // 3. Hide attack circle

        if (zonaAtaqueVisual != null)

        {

            zonaAtaqueVisual.SetActive(false);

        }



        // 4. Cooldown pause before ghost can ignite the ring again

        yield return new WaitForSeconds(1.5f);



        ejecutandoAtaque = false;

    }



    public void procesar_muerte()

    {

        StopAllCoroutines();

        if (zonaAtaqueVisual != null)

        {

            zonaAtaqueVisual.SetActive(false);

        }

        Debug.Log("[Fantasma] ¡Ha sido derrotado!");

        gameObject.SetActive(false);

    }



    bool enemigoCasiVencido() => false;

    bool EnemigoVencido() => false;

    bool jugadorTryEscape() => false;

    bool EnemigoGana() => false;



    void AlertaAGuardias() { }

    void MoviemientoJugadorDesabilitado() { }

}

