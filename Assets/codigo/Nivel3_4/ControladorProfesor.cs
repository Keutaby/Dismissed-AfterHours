
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class ControladorProfesor : MonoBehaviour
{
    public enum Estado
    {
        Oculto,
        Aviso,
        Deambular,
        Asustar
    }

    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private GameObject modeloProfesor;
    [SerializeField] private AudioSource audioRisita;
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private string tagPuntos = "punto_patrullaje";

    [Header("Tiempos aleatorios")]
    [SerializeField] private float esperaMinima = 15f;
    [SerializeField] private float esperaMaxima = 30f;
    [SerializeField] private float duracionVisibleMaxima = 20f;
    [SerializeField] private float tiempoMinimoAntesSusto = 5f;
    [SerializeField] private float tiempoMaximoAntesSusto = 12f;

    [Header("Deteccion del jugador")]
    [SerializeField] private float rangoVision = 8f;
    [SerializeField] private float distanciaDesaparicion = 1.2f;

    [Header("Velocidades")]
    [SerializeField] private float velocidadDeambular = 1.5f;
    [SerializeField] private float velocidadSusto = 8f;

    public Estado EstadoActual { get; private set; }

    private NavMeshAgent agent;
    private Transform camara;
    private float tiempoDisponibleSusto;
    private int ultimoPunto = -1;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (audioRisita == null)
            audioRisita = GetComponent<AudioSource>();

        if (modeloProfesor != null)
            modeloProfesor.SetActive(false);
    }

    void Start()
    {
        // Buscar al jugador
        if (jugador == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("jugador");

            if (obj != null)
                jugador = obj.transform;
        }

        // Buscar la camara del jugador
        if (Camera.main != null)
            camara = Camera.main.transform;

        // Buscar puntos de patrulla
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
        {
            GameObject[] puntos =
                GameObject.FindGameObjectsWithTag(tagPuntos);

            puntosPatrulla = new Transform[puntos.Length];

            for (int i = 0; i < puntos.Length; i++)
            {
                puntosPatrulla[i] = puntos[i].transform;
            }
        }

        CambiarEstado(Estado.Oculto);

    }

    private bool cicloIniciado = false;
    public void IniciarAcecho()
    {
        if (cicloIniciado)
            return;

        cicloIniciado = true;
        StartCoroutine(CicloApariciones());
    }


    void Update()
    {
        if (jugador == null)
            return;

        float distancia = DistanciaAlJugador();

        switch (EstadoActual)
        {
            case Estado.Deambular:

                Patrullar();

                // El profesor ya puede intentar asustar
                if (Time.time >= tiempoDisponibleSusto &&
                    distancia <= rangoVision)
                {
                    CambiarEstado(Estado.Asustar);
                }

                break;

            case Estado.Asustar:

                // Correr directamente hacia el jugador
                agent.SetDestination(PosicionObjetivo());

                // Desaparecer al llegar hasta el jugador
                if (distancia <= distanciaDesaparicion)
                {
                    CambiarEstado(Estado.Oculto);
                }

                break;
        }
    }


    private IEnumerator CicloApariciones()
    {
        while (true)
        {
            // 1. Esperar oculto durante un tiempo aleatorio
            CambiarEstado(Estado.Oculto);

            float espera = Random.Range(
                esperaMinima,
                esperaMaxima
            );

            yield return new WaitForSeconds(espera);

            // 2. Reproducir la risita
            CambiarEstado(Estado.Aviso);

            if (audioRisita != null)
            {
                audioRisita.Play();

                yield return new WaitWhile(
                    () => audioRisita.isPlaying
                );
            }

            // 3. Aparecer en un lugar aleatorio
            if (!AparecerEnPuntoAleatorio())
            {
                yield return new WaitForSeconds(2f);
                continue;
            }

            if (modeloProfesor != null)
                modeloProfesor.SetActive(true);

            CambiarEstado(Estado.Deambular);

            // Esperar un tiempo aleatorio antes de poder asustar
            tiempoDisponibleSusto = Time.time + Random.Range(
                tiempoMinimoAntesSusto,
                tiempoMaximoAntesSusto
            );

            // 4. Permanecer visible hasta desaparecer o agotar el tiempo
            float finVisibilidad = Time.time + duracionVisibleMaxima;

            while (EstadoActual != Estado.Oculto &&
                   Time.time < finVisibilidad)
            {
                yield return null;
            }

            // Si sigue visible, desaparecer
            if (EstadoActual != Estado.Oculto)
            {
                CambiarEstado(Estado.Oculto);
            }
        }
    }

    private void Patrullar()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            ElegirNuevoPunto();
        }
    }

    private void ElegirNuevoPunto()
    {
        if (puntosPatrulla.Length == 0)
            return;

        int indice = Random.Range(0, puntosPatrulla.Length);

        // Evitar elegir el mismo punto dos veces seguidas
        if (puntosPatrulla.Length > 1)
        {
            while (indice == ultimoPunto)
            {
                indice = Random.Range(0, puntosPatrulla.Length);
            }
        }

        ultimoPunto = indice;

        agent.SetDestination(puntosPatrulla[indice].position);
    }

    private bool AparecerEnPuntoAleatorio()
{
    if (puntosPatrulla == null || puntosPatrulla.Length == 0)
        return false;

    int indice = Random.Range(0, puntosPatrulla.Length);

    // Evitar aparecer dos veces seguidas en el mismo lugar
    if (puntosPatrulla.Length > 1)
    {
        while (indice == ultimoPunto)
        {
            indice = Random.Range(0, puntosPatrulla.Length);
        }
    }

    // Teletransportar al profesor al punto elegido
    bool aparecio = agent.Warp(puntosPatrulla[indice].position);

    if (aparecio)
    {
        ultimoPunto = indice;
        return true;
    }

    Debug.LogWarning("No se pudo colocar al profesor en el punto elegido.");
    return false;
}

    private void CambiarEstado(Estado nuevo)
    {
        EstadoActual = nuevo;

        switch (nuevo)
        {
            case Estado.Oculto:

                agent.isStopped = true;
                agent.ResetPath();

                if (modeloProfesor != null)
                    modeloProfesor.SetActive(false);

                break;

            case Estado.Aviso:

                agent.isStopped = true;
                agent.ResetPath();

                break;

            case Estado.Deambular:

                agent.isStopped = false;
                agent.speed = velocidadDeambular;
                agent.stoppingDistance = 0f;

                ElegirNuevoPunto();

                break;

            case Estado.Asustar:

                agent.isStopped = false;
                agent.speed = velocidadSusto;
                agent.stoppingDistance = 0f;

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
}
