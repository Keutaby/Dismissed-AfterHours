using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SistemaSalud))]
public class GestorMuerte : MonoBehaviour
{
    [Header("UI de Muerte")]
    [SerializeField] private GameObject canvasMuerte;      // el CanvasMuerte (puede empezar apagado)
    [SerializeField] private CanvasGroup fondo;            // opcional: fondo negro
    [SerializeField] private CanvasGroup texto1;
    [SerializeField] private CanvasGroup texto2;
    [SerializeField] private CanvasGroup botonReiniciar;   // el CanvasGroup del botón (o su padre)

    [Header("Tiempos")]
    [SerializeField] private float duracionFondo = 1f;
    [SerializeField] private float duracionFade = 0.6f;
    [SerializeField] private float tiempoVisible = 1.5f;

    [Header("Opcional")]
    [SerializeField] private bool pausarJuego = true;      // congela fantasmas mientras sale el mensaje
    [SerializeField] private AudioSource sonidoMuerte;

    private SistemaSalud sistemaSalud;
    private bool yaMuerto;

    void Awake() => sistemaSalud = GetComponent<SistemaSalud>();

    void OnEnable() => sistemaSalud.alMorir += IniciarMuerte;
    void OnDisable() => sistemaSalud.alMorir -= IniciarMuerte;

    void Start()
    {
        Time.timeScale = 1f;   // por si la escena anterior quedó pausada
        if (canvasMuerte != null) canvasMuerte.SetActive(false);
    }

    private void IniciarMuerte()
    {
        if (yaMuerto) return;
        yaMuerto = true;
        StartCoroutine(SecuenciaMuerte());
    }

    private IEnumerator SecuenciaMuerte()
    {
        if (sonidoMuerte != null) sonidoMuerte.Play();

        // Todo invisible antes de encender el canvas
        Ocultar(fondo);
        Ocultar(texto1);
        Ocultar(texto2);
        Ocultar(botonReiniciar);

        if (canvasMuerte != null) canvasMuerte.SetActive(true);
        if (pausarJuego) Time.timeScale = 0f;

        yield return Fade(fondo, 0f, 1f, duracionFondo);
        yield return new WaitForSecondsRealtime(0.3f);

        yield return Fade(texto1, 0f, 1f, duracionFade);
        yield return new WaitForSecondsRealtime(tiempoVisible);
        yield return Fade(texto1, 1f, 0f, duracionFade);

        yield return new WaitForSecondsRealtime(0.3f);

        yield return Fade(texto2, 0f, 1f, duracionFade);
        yield return new WaitForSecondsRealtime(tiempoVisible);

        // Al final aparece el botón y ya se puede pulsar
        if (botonReiniciar != null)
        {
            botonReiniciar.blocksRaycasts = true;
            botonReiniciar.interactable = true;
            yield return Fade(botonReiniciar, 0f, 1f, duracionFade);
        }
    }

    private void Ocultar(CanvasGroup g)
    {
        if (g == null) return;
        g.alpha = 0f;
        g.blocksRaycasts = false;
        g.interactable = false;
    }

    // Usa tiempo real, así funciona aunque el juego esté pausado
    private IEnumerator Fade(CanvasGroup g, float inicio, float fin, float duracion)
    {
        if (g == null) yield break;

        float t = 0f;
        g.alpha = inicio;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            g.alpha = Mathf.Lerp(inicio, fin, t / duracion);
            yield return null;
        }
        g.alpha = fin;
    }

    // La llama el botón desde el Inspector
    public void BotonReiniciar()
    {
        Time.timeScale = 1f;   // si no, la escena nueva carga congelada
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}