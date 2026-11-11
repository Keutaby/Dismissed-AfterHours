using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TextoAlMirar : MonoBehaviour
{
    [Header("Texto de ESTE objeto")]
    [SerializeField] private GameObject texto;          // el TMP propio de este objeto

    [Header("Cuándo se ve")]
    [SerializeField] private float distanciaMaxima = 4f;
    [SerializeField] private float anguloMirada = 25f;
    [SerializeField] private bool ocultarAlAgarrar = true;

    [Header("Opcional")]
    [SerializeField] private bool mirarALaCamara = true;
    [SerializeField] private float velocidadFundido = 4f;

    // Coordinación: solo se muestra el texto del objeto más centrado en tu mirada
    private static readonly List<TextoAlMirar> todos = new List<TextoAlMirar>();
    private static int frameCalculado = -1;
    private static TextoAlMirar elegido;

    private XRGrabInteractable grab;     // puede ser null si el objeto no se agarra
    private CanvasGroup grupo;           // el del PROPIO texto (se crea solo si falta)
    private Transform camara;
    private bool yaAgarrado;

    void Awake() => grab = GetComponent<XRGrabInteractable>();

    void OnEnable()
    {
        todos.Add(this);
        if (grab != null) grab.selectEntered.AddListener(OnAgarrado);
    }

    void OnDisable()
    {
        todos.Remove(this);
        if (elegido == this) elegido = null;
        if (grab != null) grab.selectEntered.RemoveListener(OnAgarrado);
    }

    void Start()
    {
        if (Camera.main != null) camara = Camera.main.transform;
        if (texto == null) return;

        grupo = texto.GetComponent<CanvasGroup>();
        if (grupo == null) grupo = texto.AddComponent<CanvasGroup>();

        grupo.alpha = 0f;
        texto.SetActive(false);
    }

    private void OnAgarrado(SelectEnterEventArgs args)
    {
        if (ocultarAlAgarrar) yaAgarrado = true;
    }

    void Update()
    {
        if (texto == null || camara == null || grupo == null) return;

        ElegirGanador();
        bool visible = elegido == this;

        if (visible && !texto.activeSelf) texto.SetActive(true);

        grupo.alpha = Mathf.MoveTowards(grupo.alpha, visible ? 1f : 0f, velocidadFundido * Time.unscaledDeltaTime);

        if (!visible && grupo.alpha <= 0.01f)
        {
            texto.SetActive(false);
            return;
        }

        if (mirarALaCamara)
            texto.transform.rotation = Quaternion.LookRotation(texto.transform.position - camara.position);
    }

    private static void ElegirGanador()
    {
        if (frameCalculado == Time.frameCount) return;
        frameCalculado = Time.frameCount;
        elegido = null;
        float mejorAngulo = float.MaxValue;

        foreach (TextoAlMirar t in todos)
        {
            if (t.camara == null || t.yaAgarrado) continue;

            Vector3 haciaObjeto = t.transform.position - t.camara.position;
            if (haciaObjeto.magnitude > t.distanciaMaxima) continue;

            float angulo = Vector3.Angle(t.camara.forward, haciaObjeto);
            if (angulo > t.anguloMirada) continue;

            if (angulo < mejorAngulo)
            {
                mejorAngulo = angulo;
                elegido = t;
            }
        }
    }
}