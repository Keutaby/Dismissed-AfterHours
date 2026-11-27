using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class PoliciaFantasma : MonoBehaviour
{
    public enum Estado
    {
        Patrullar,
        Perseguir,
        Atacar,
        Aturdido
    }

    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private GameObject zonaAtaque;
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private string tagPuntos = "punto_patrullaje";

    [Header("Rangos")]
    [SerializeField] private float rangoVision = 8f;
    [SerializeField] private float rangoPerdida = 12f;
    [SerializeField] private float rangoAtaque = 2.5f;
    [SerializeField] private LayerMask capasObstaculos;


    [Header("Velocidades")]
    [SerializeField] private float velocidadPatrulla = 1.5f;
    [SerializeField] private float velocidadPersecucion = 3f;

    [Header("Aturdimiento")]
    [SerializeField] private float duracionAturdimiento = 12f;

    public Estado EstadoActual {get; private set;}

    private NavMeshAgent agent;
    private Transform camara;
    private float finAturdimiento;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = rangoAtaque * 0.8f;

        if(zonaAtaque != null)
            zonaAtaque.SetActive(false);
    } 
    void Start()
    {
        if(jugador == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("jugador");

            if(obj != null)
            jugador = obj.transform;

        }

        if (Camera.main != null)
            camara = Camera.main.transform;

        if(puntosPatrulla == null || puntosPatrulla.Length == 0)
        {
            GameObject[] puntos = GameObject.FindGameObjectsWithTag(tagPuntos);
            puntosPatrulla = new Transform[puntos.Length];

            for (int i=0; i< puntos.Length; i++)
            {
                puntosPatrulla[i] = puntos[i].transform;
            }
        }

        CambiarEstado(Estado.Patrullar);
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = DistanciaAlJugador();

        switch (EstadoActual)
        {
            case Estado.Patrullar:

                Patrullar();

                if (distancia <= rangoVision && VeAlJugador())
                {
                    CambiarEstado(Estado.Perseguir);
                }

                break;


            case Estado.Perseguir:

                if (distancia > rangoPerdida)
                {
                    CambiarEstado(Estado.Patrullar);
                }
                else if (distancia <= rangoAtaque)
                {
                    CambiarEstado(Estado.Atacar);
                }
                else
                {
                    agent.SetDestination(PosicionObjetivo());
                }

                break;


            case Estado.Atacar:

                MirarAlJugador();

                if (distancia > rangoAtaque + 0.5f)
                {
                    CambiarEstado(Estado.Perseguir);
                }

                break;


            case Estado.Aturdido:

                if (Time.time >= finAturdimiento)
                {
                    if (distancia <= rangoPerdida)
                    {
                        CambiarEstado(Estado.Perseguir);
                    }
                    else
                    {
                        CambiarEstado(Estado.Patrullar);
                    }
                }

                break;
        }
    }

    private void Patrullar()
    {
        if(puntosPatrulla.Length == 0) return;

        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            ElegirNuevoPunto();
        }
    }

    private void ElegirNuevoPunto()
    {
        if (puntosPatrulla.Length == 0) return;
        int indice = Random.Range(0, puntosPatrulla.Length);
        agent.SetDestination(puntosPatrulla[indice].position);
    }

    private void CambiarEstado(Estado nuevo)
    {
        EstadoActual = nuevo;

        switch (nuevo)
        {
            case Estado.Patrullar:
                agent.isStopped = false;
                agent.speed = velocidadPatrulla;

                if (zonaAtaque != null)
                    zonaAtaque.SetActive(false);

                ElegirNuevoPunto();
                break;

            case Estado.Perseguir:
                agent.isStopped = false;
                agent.speed = velocidadPersecucion;

                if (zonaAtaque != null)
                    zonaAtaque.SetActive(false);
                break;

            case Estado.Atacar:
                agent.isStopped = true;

                if (zonaAtaque != null)
                    zonaAtaque.SetActive(true);
                break;

            case Estado.Aturdido:
                agent.isStopped = true;
                agent.ResetPath();

                if (zonaAtaque != null)
                    zonaAtaque.SetActive(false);
                break;
        }
    }


    private Vector3 PosicionJugador()
    {
        return camara != null ? camara.position : jugador.position;
    }

    private Vector3 PosicionObjetivo()
    {
        Vector3 posicion = PosicionJugador();

        return new Vector3(
            posicion.x,
            transform.position.y,
            posicion.z
        );
    }

    private float DistanciaAlJugador()
    {
        return Vector3.Distance(
            transform.position,
            PosicionObjetivo()
        );
    }

    private void MirarAlJugador()
    {
        Vector3 direccion = PosicionObjetivo() - transform.position;

        direccion.y = 0;

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                Time.deltaTime * 5f
            );
        }
    }

    public void RecibirGolpeRegla()
    { 

        finAturdimiento = Time.time + duracionAturdimiento;

        CambiarEstado(Estado.Aturdido);
    }     

    private bool VeAlJugador()
    {
        Vector3 origen = transform.position + Vector3.up;

        return !Physics.Linecast(
            origen,
            PosicionJugador(),
            capasObstaculos
        );
    }

}