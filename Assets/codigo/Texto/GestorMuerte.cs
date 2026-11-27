using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SistemaSalud))]
public class GestorMuerte : MonoBehaviour
{
    [Header("UI de Muerte")]
    [SerializeField] private GameObject canvasMuerte;
    [SerializeField] private CanvasGroup fondo;
    [SerializeField] private CanvasGroup texto1;
    [SerializeField] private CanvasGroup texto2;
    [SerializeField] private CanvasGroup botonReiniciar;

    [Header("Pantalla negra total")]
    [SerializeField] private bool apagarElMundo = true;
    [SerializeField] private LayerMask capasVisiblesAlMorir;   // opcional: capa de los mandos/rayos
    [SerializeField] private bool canvasSigueCabeza = true;
    [SerializeField] private float distanciaCanvas = 1.5f;

    [Header("Luz del botón")]
    [SerializeField] private Light luzBoton;        // la luz que metiste dentro del botón
    [SerializeField] private float intensidadLuz = 2f;

    [Header("Tiempos")]
    [SerializeField] private float duracionFondo = 1f;
    [SerializeField] private float duracionFade = 0.6f;
    [SerializeField] private float tiempoVisible = 1.5f;

    [Header("Opcional")]
    [SerializeField] private bool pausarJuego = true;
    [SerializeField] private AudioSource sonidoMuerte;

    // --- Bandera "vengo de una muerte": solo vale en la escena que se reinició ---
    private static int escenaReiniciada = -2;
    public static bool ReinicioPorMuerte
    {
        get => escenaReiniciada == SceneManager.GetActiveScene().buildIndex;
        set => escenaReiniciada = value ? SceneManager.GetActiveScene().buildIndex : -2;
    }

    private SistemaSalud sistemaSalud;
    private bool yaMuerto;

    void Awake() => sistemaSalud = GetComponent<SistemaSalud>();

    void OnEnable() => sistemaSalud.alMorir += IniciarMuerte;
    void OnDisable() => sistemaSalud.alMorir -= IniciarMuerte;

    void Start()
    {
        Time.timeScale = 1f;
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
        if (luzBoton != null) luzBoton.enabled = false;
        if (sonidoMuerte != null) sonidoMuerte.Play();

        Ocultar(fondo);
        Ocultar(texto1);
        Ocultar(texto2);
        Ocultar(botonReiniciar);

        if (apagarElMundo) ApagarMundo();
        if (canvasMuerte != null)
        {
            if (canvasSigueCabeza) AnclarCanvasALaCabeza();
            canvasMuerte.SetActive(true);
        }
        if (pausarJuego) Time.timeScale = 0f;

        yield return Fade(fondo, 0f, 1f, duracionFondo);
        yield return new WaitForSecondsRealtime(0.3f);

        yield return Fade(texto1, 0f, 1f, duracionFade);
        yield return new WaitForSecondsRealtime(tiempoVisible);
        yield return Fade(texto1, 1f, 0f, duracionFade);

        yield return new WaitForSecondsRealtime(0.3f);

        yield return Fade(texto2, 0f, 1f, duracionFade);
        yield return new WaitForSecondsRealtime(tiempoVisible);

        if (botonReiniciar != null)
            yield return AparecerBoton();
    }

    // La cámara solo dibuja la UI sobre fondo negro: el edificio desaparece
    private void ApagarMundo()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        int mascara = capasVisiblesAlMorir.value;
        mascara |= 1 << LayerMask.NameToLayer("UI");
        if (canvasMuerte != null) mascara |= 1 << canvasMuerte.layer;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.cullingMask = mascara;
    }

    private void AnclarCanvasALaCabeza()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Transform t = canvasMuerte.transform;
        t.SetParent(cam.transform, false);
        t.localPosition = new Vector3(0f, 0f, distanciaCanvas);
        t.localRotation = Quaternion.identity;
    }

    private void Ocultar(CanvasGroup g)
    {
        if (g == null) return;
        g.alpha = 0f;
        g.blocksRaycasts = false;
        g.interactable = false;
    }

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
    // El botón y su luz aparecen juntos
    private IEnumerator AparecerBoton()
    {
        botonReiniciar.blocksRaycasts = true;
        botonReiniciar.interactable = true;

        if (luzBoton != null)
        {
            luzBoton.intensity = 0f;
            luzBoton.enabled = true;
        }

        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duracionFade);

            botonReiniciar.alpha = k;
            if (luzBoton != null) luzBoton.intensity = Mathf.Lerp(0f, intensidadLuz, k);

            yield return null;
        }

        botonReiniciar.alpha = 1f;
        if (luzBoton != null) luzBoton.intensity = intensidadLuz;
    }
    // La llama el botón desde el Inspector
    public void BotonReiniciar()
    {
        ReinicioPorMuerte = true;   // la escena nueva sabrá que viene de una muerte
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}