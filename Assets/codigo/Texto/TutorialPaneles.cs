using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialPaneles : MonoBehaviour
{
    [System.Serializable]
    public class Pagina
    {
        public Sprite imagen;
        [TextArea(3, 8)] public string texto;
    }

    [SerializeField] private Controlador_Intro intro;   // se inicia al terminar el tutorial

    [Header("Pantalla")]
    [SerializeField] private Image pantalla;
    [SerializeField] private TMP_Text textoUI;
    [SerializeField] private Pagina[] paginas;

    [Header("Máquina de escribir")]
    [SerializeField] private float letrasPorSegundo = 35f;
    [SerializeField] private float pausaPuntuacion = 0.15f;   // pausa extra en . , ! ? :
    [SerializeField] private AudioSource sonidoLetra;         // opcional: "tic" por letra

    [Header("Control")]
    [SerializeField] private InputActionReference botonSiguiente;   // XRI Right Interaction/Activate
    [SerializeField] private float pausaEntrePulsaciones = 0.25f;

    [Header("Pausa")]
    [SerializeField] private bool silenciarAudioDelJuego = true;      // pausa el resto del audio
    [SerializeField] private AudioSource[] audiosQueSiSuenan;         // otros sonidos que no se deben silenciar
    [SerializeField] private GameObject[] desactivarDuranteTutorial;  // opcional: por ejemplo Locomotion
    [SerializeField] private Behaviour[] scriptsDesactivar;           // opcional: FlashMecanica, ReglaT

    [Header("Opcional")]
    [SerializeField] private AudioSource sonidoPagina;
    [SerializeField] private UnityEvent alTerminar;

    private int actual;
    private bool escribiendo;
    private Coroutine rutina;
    private float siguientePermitido;

    void Awake()
    {
        // Estos sonidos ignoran la pausa de audio, así que se oyen aunque lo demás esté callado
        if (sonidoLetra != null) sonidoLetra.ignoreListenerPause = true;
        if (sonidoPagina != null) sonidoPagina.ignoreListenerPause = true;

        foreach (var a in audiosQueSiSuenan)
            if (a != null) a.ignoreListenerPause = true;
    }

    void OnEnable()
    {
        if (botonSiguiente == null) return;
        botonSiguiente.action.Enable();
        botonSiguiente.action.performed += OnBoton;
    }

    void OnDisable()
    {
        if (botonSiguiente != null) botonSiguiente.action.performed -= OnBoton;
        Pausar(false);   // siempre restaura el tiempo, aunque se cierre de golpe
    }

    void Start()
    {
        Pausar(true);
        actual = 0;
        MostrarPagina();
    }

    private void OnBoton(InputAction.CallbackContext ctx)
    {
        if (Time.unscaledTime < siguientePermitido) return;
        siguientePermitido = Time.unscaledTime + pausaEntrePulsaciones;

        if (escribiendo)
        {
            // Primer toque: completa el texto al instante
            if (rutina != null) StopCoroutine(rutina);
            textoUI.maxVisibleCharacters = int.MaxValue;
            escribiendo = false;
            return;
        }

        // Segundo toque: siguiente página
        if (sonidoPagina != null) sonidoPagina.Play();
        actual++;
        if (actual >= paginas.Length) Terminar();
        else MostrarPagina();
    }

    private void MostrarPagina()
    {
        Pagina p = paginas[actual];
        if (pantalla != null) pantalla.sprite = p.imagen;

        textoUI.text = p.texto;
        textoUI.maxVisibleCharacters = 0;

        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(Escribir());
    }

    private IEnumerator Escribir()
    {
        escribiendo = true;
        textoUI.ForceMeshUpdate();
        int total = textoUI.textInfo.characterCount;   // ignora etiquetas <b>, <color>, etc.
        float espera = 1f / Mathf.Max(letrasPorSegundo, 1f);

        for (int i = 1; i <= total; i++)
        {
            textoUI.maxVisibleCharacters = i;

            char c = textoUI.textInfo.characterInfo[i - 1].character;
            if (sonidoLetra != null && !char.IsWhiteSpace(c)) sonidoLetra.Play();

            bool puntuacion = c == '.' || c == ',' || c == '!' || c == '?' || c == ':';
            yield return new WaitForSecondsRealtime(espera + (puntuacion ? pausaPuntuacion : 0f));
        }

        escribiendo = false;
    }

    private void Pausar(bool pausa)
    {
        Time.timeScale = pausa ? 0f : 1f;
        if (silenciarAudioDelJuego) AudioListener.pause = pausa;

        foreach (var o in desactivarDuranteTutorial)
            if (o != null) o.SetActive(!pausa);

        foreach (var s in scriptsDesactivar)
            if (s != null) s.enabled = !pausa;
    }

    private void Terminar()
    {
        Debug.Log("[Tutorial] Terminar. Intro asignada: " + (intro != null));
        Pausar(false);                              // primero restaura el tiempo
        if (intro != null) intro.IniciarIntro();    // luego dispara la intro
        alTerminar?.Invoke();
        gameObject.SetActive(false);
    }
}