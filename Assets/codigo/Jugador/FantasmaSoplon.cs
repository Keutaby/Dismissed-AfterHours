using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FantasmaSoplon : MonoBehaviour, MonitorMuerte
{
    public enum Estado { Patrullar, Perseguir, Atacar, Aturdido, Muerto }
    [Header("Referencias")]
    [SerializeField] private Transform jugador;          // si lo dejas vacío, busca el tag "jugador"
    [SerializeField] private GameObject zonaAtaque;      // el círculo con ZonaDanoFantasma
    [SerializeField] private Transform[] puntosPatrulla; // si lo dejas vacío, busca el tag "punto_patrullaje"
    [SerializeField] private string tagPuntos = "punto_patrullaje";


    [Header("Rangos")]
    [SerializeField] private float rangoVision = 8f;     // a esta distancia te detecta
    [SerializeField] private float rangoPerdida = 12f;   // más lejos que esto te pierde (mayor que rangoVision)
    [SerializeField] private float rangoAtaque = 2.5f;   // iguálalo al radio de tu círculo de ataque
    [SerializeField] private LayerMask capasObstaculos;  // paredes. Déjalo en Nothing si no quieres que bloqueen la vista

    [Header("Velocidades")]
    [SerializeField] private float velocidadPatrulla = 1.5f;
    [SerializeField] private float velocidadPersecucion = 3f;

    [Header("Resistencia al flash")]
    [SerializeField] private float multiplicadorDanoFlash = 1f;       // 1 = daño normal, 0.5 = la mitad
    [SerializeField] private float multiplicadorAturdimiento = 1f;    // 0.5 = se aturde la mitad del tiempo

    [Header("Resistencia a la regla T")]
    [SerializeField] private float multiplicadorDanoRegla = 1f;
    public float MultiplicadorDanoRegla => multiplicadorDanoRegla;

    public float MultiplicadorDanoFlash => multiplicadorDanoFlash;
    public float MultiplicadorAturdimiento => multiplicadorAturdimiento;

    public Estado EstadoActual { get; private set; }

    private NavMeshAgent agent;
    private Transform camara;
    private float finAturdimiento;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = rangoAtaque * 0.8f; // se detiene antes de llegar encima de ti

        if (zonaAtaque != null) zonaAtaque.SetActive(false);
    }

    void Start()
    {
        if (jugador == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("jugador");
            if (obj != null) jugador = obj.transform;
        }

        if (Camera.main != null) camara = Camera.main.transform; // la cabeza del jugador en VR

        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
        {
            GameObject[] puntos = GameObject.FindGameObjectsWithTag(tagPuntos);
            puntosPatrulla = new Transform[puntos.Length];
            for (int i = 0; i < puntos.Length; i++) puntosPatrulla[i] = puntos[i].transform;
        }

        CambiarEstado(Estado.Patrullar);
    }

    void Update()
    {
        if (EstadoActual == Estado.Muerto || jugador == null) return;

        float distancia = DistanciaAlJugador();

        switch (EstadoActual)
        {
            case Estado.Aturdido:
                if (Time.time >= finAturdimiento)
                    CambiarEstado(Estado.Perseguir);
                break;
            case Estado.Patrullar:
                Patrullar();
                if (distancia <= rangoVision && VeAlJugador())
                    CambiarEstado(Estado.Perseguir);
                break;

            case Estado.Perseguir:
                if (distancia > rangoPerdida)
                    CambiarEstado(Estado.Patrullar);
                else if (distancia <= rangoAtaque)
                    CambiarEstado(Estado.Atacar);
                else
                    agent.SetDestination(PosicionObjetivo());
                break;

            case Estado.Atacar:
                MirarAlJugador();
                if (distancia > rangoAtaque + 0.5f) // margen para que no parpadee entre estados
                    CambiarEstado(Estado.Perseguir);
                break;
        }
    }

    // ---------- Estados ----------

    private void CambiarEstado(Estado nuevo)
    {
        EstadoActual = nuevo;

        switch (nuevo)
        {
            case Estado.Patrullar:
                agent.isStopped = false;
                agent.speed = velocidadPatrulla;
                if (zonaAtaque != null) zonaAtaque.SetActive(false);
                ElegirNuevoPunto();
                break;

            case Estado.Perseguir:
                agent.isStopped = false;
                agent.speed = velocidadPersecucion;
                if (zonaAtaque != null) zonaAtaque.SetActive(false);
                break;

            case Estado.Atacar:
                agent.isStopped = true;
                if (zonaAtaque != null) zonaAtaque.SetActive(true);
                break;

            case Estado.Aturdido:
                agent.isStopped = true;
                if (zonaAtaque != null) zonaAtaque.SetActive(false);
                break;

            case Estado.Muerto:
                agent.isStopped = true;
                if (zonaAtaque != null) zonaAtaque.SetActive(false);
                break;
        }
    }

    private void Patrullar()
    {
        if (puntosPatrulla.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            ElegirNuevoPunto();
    }
    private void ElegirNuevoPunto()
    {
        if (puntosPatrulla.Length == 0) return;
        agent.SetDestination(puntosPatrulla[Random.Range(0, puntosPatrulla.Length)].position);
    }

    // ---------- Detección ----------

    // Posición de la cabeza del jugador (en VR el XR Origin no siempre está donde estás tú)
    private Vector3 PosicionJugador()
    {
        return camara != null ? camara.position : jugador.position;
    }

    // Misma altura que el fantasma, para que la distancia sea "en el piso"
    private Vector3 PosicionObjetivo()
    {
        Vector3 p = PosicionJugador();
        return new Vector3(p.x, transform.position.y, p.z);
    }

    private float DistanciaAlJugador()
    {
        return Vector3.Distance(transform.position, PosicionObjetivo());
    }

    private bool VeAlJugador()
    {
        Vector3 origen = transform.position + Vector3.up * 1f;
        return !Physics.Linecast(origen, PosicionJugador(), capasObstaculos);
    }

    private void MirarAlJugador()
    {
        Vector3 dir = PosicionObjetivo() - transform.position;
        if (dir.sqrMagnitude < 0.01f) return;
        Quaternion giro = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, giro, 8f * Time.deltaTime);
    }

    // ---------- Muerte (la usa SistemaSalud por ahora) ----------

    public void Aturdir(float segundos)
    {
        if (EstadoActual == Estado.Muerto) return;

        finAturdimiento = Time.time + segundos;
        CambiarEstado(Estado.Aturdido);
    }
    public void procesar_muerte()
    {
        CambiarEstado(Estado.Muerto);
        gameObject.SetActive(false);
    }
}