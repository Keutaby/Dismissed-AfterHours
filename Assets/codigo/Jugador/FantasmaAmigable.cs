using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FantasmaAmigable : MonoBehaviour
{
    private enum Fase { Esperando, DialogoInicio, Ordas, EsperandoRegreso, DialogoFinal, Terminado }

    [Header("Referencias")]
    [SerializeField] private GameObject panelDialogo;     // el panel con el texto (empieza apagado)
    [SerializeField] private TMP_Text textoUI;
    [SerializeField] private OrdasController ordas;       // desmarca "Iniciar Al Empezar" en ese script

    [Header("Diálogos (una línea por elemento)")]
    [SerializeField, TextArea(2, 5)] private string[] dialogoInicio;
    [SerializeField, TextArea(2, 5)] private string[] dialogoFinal;

    [Header("Activación")]
    [SerializeField] private float distanciaActivacion = 3f;
    [SerializeField] private bool mirarAlJugador = true;

    [Header("Máquina de escribir")]
    [SerializeField] private float letrasPorSegundo = 30f;
    [SerializeField] private float pausaPuntuacion = 0.12f;
    [SerializeField] private AudioSource sonidoLetra;     // opcional

    [Header("Avanzar")]
    [SerializeField] private InputActionReference botonSiguiente;
    [SerializeField] private float segundosAutoAvance = 0f;   // 0 = solo con el botón; 3 = avanza solo a los 3 s

    [Header("Final")]
    [SerializeField] private string escenaSiguiente = "Nivel4";
    [SerializeField] private float retrasoAntesDeCargar = 1.5f;

    [Header("Eventos opcionales")]
    [SerializeField] private UnityEvent alTerminarInicio;
    [SerializeField] private UnityEvent alTerminarFinal;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerCelebrar = "Celebrar";   // debe llamarse igual que en el Animator

    private Fase fase = Fase.Esperando;
    private Transform camara;
    private bool dialogoActivo;
    private bool pulsado;
    private float siguientePermitido;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (Camera.main != null) camara = Camera.main.transform;
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (ordas != null) ordas.todasCompletadas += OnOrdasCompletas;
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
    }

    void OnDestroy()
    {
        if (ordas != null) ordas.todasCompletadas -= OnOrdasCompletas;
    }

    void Update()
    {
        if (camara == null) return;

        if (mirarAlJugador && fase != Fase.Ordas)
        {
            Vector3 dir = camara.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 4f * Time.deltaTime);
        }

        if (panelDialogo != null && panelDialogo.activeSelf)
            panelDialogo.transform.rotation = Quaternion.LookRotation(panelDialogo.transform.position - camara.position);

        if (fase == Fase.Esperando && JugadorCerca())
            StartCoroutine(FlujoInicio());
        else if (fase == Fase.EsperandoRegreso && JugadorCerca())
            StartCoroutine(FlujoFinal());
    }

    private bool JugadorCerca()
    {
        Vector3 d = camara.position - transform.position;
        d.y = 0f;   // solo distancia en el piso
        return d.magnitude <= distanciaActivacion;
    }

    // ---------- Flujo ----------

    private IEnumerator FlujoInicio()
    {
        fase = Fase.DialogoInicio;
        yield return Reproducir(dialogoInicio);
        alTerminarInicio?.Invoke();

        if (ordas != null)
        {
            fase = Fase.Ordas;
            ordas.IniciarOrdas();
        }
        else
        {
            Debug.LogWarning("[Fantasma] No hay OrdasController asignado");
            fase = Fase.EsperandoRegreso;
        }
    }

    private void OnOrdasCompletas()
    {
        if (fase == Fase.Ordas) fase = Fase.EsperandoRegreso;   // el diálogo final sale cuando estés cerca
    }

    private IEnumerator FlujoFinal()
    {
        fase = Fase.DialogoFinal;
        if (animator != null) animator.SetTrigger(triggerCelebrar);
        yield return Reproducir(dialogoFinal);
        alTerminarFinal?.Invoke();

        fase = Fase.Terminado;
        yield return new WaitForSeconds(retrasoAntesDeCargar);

        PlayerPrefs.SetString("UltimaEscena", escenaSiguiente);   // ← guarda el progreso
        PlayerPrefs.Save();
        SceneManager.LoadScene(escenaSiguiente);
    }
    // ---------- Diálogo ----------

    private IEnumerator Reproducir(string[] lineas)
    {
        dialogoActivo = true;
        if (panelDialogo != null) panelDialogo.SetActive(true);

        foreach (string linea in lineas)
        {
            yield return Escribir(linea);
            yield return EsperarSiguiente();
        }

        if (panelDialogo != null) panelDialogo.SetActive(false);
        dialogoActivo = false;
    }

    private IEnumerator Escribir(string linea)
    {
        textoUI.text = linea;
        textoUI.maxVisibleCharacters = 0;
        textoUI.ForceMeshUpdate();

        int total = textoUI.textInfo.characterCount;
        float espera = 1f / Mathf.Max(letrasPorSegundo, 1f);
        pulsado = false;

        for (int i = 1; i <= total; i++)
        {
            if (pulsado)   // un toque completa la línea al instante
            {
                pulsado = false;
                break;
            }

            textoUI.maxVisibleCharacters = i;

            char c = textoUI.textInfo.characterInfo[i - 1].character;
            if (sonidoLetra != null && !char.IsWhiteSpace(c)) sonidoLetra.Play();

            bool puntuacion = c == '.' || c == ',' || c == '!' || c == '?' || c == ':';
            yield return new WaitForSeconds(espera + (puntuacion ? pausaPuntuacion : 0f));
        }

        textoUI.maxVisibleCharacters = int.MaxValue;
    }

    private IEnumerator EsperarSiguiente()
    {
        pulsado = false;
        float t = 0f;

        while (!pulsado)
        {
            t += Time.deltaTime;
            if (segundosAutoAvance > 0f && t >= segundosAutoAvance) break;
            yield return null;
        }

        pulsado = false;
    }

    private void OnBoton(InputAction.CallbackContext ctx)
    {
        if (!dialogoActivo) return;
        if (Time.unscaledTime < siguientePermitido) return;
        siguientePermitido = Time.unscaledTime + 0.25f;
        pulsado = true;
    }
}