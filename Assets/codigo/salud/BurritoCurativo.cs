using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BurritoCurativo : MonoBehaviour
{
    [SerializeField] private int curacion = 30;
    [SerializeField] private AudioSource sonido;
    [SerializeField] private float tiempoAntesDeDesaparecer = 0.6f;

    private XRGrabInteractable grab;
    private bool consumido;

    void Awake() => grab = GetComponent<XRGrabInteractable>();
    void OnEnable() => grab.selectEntered.AddListener(OnAgarrado);
    void OnDisable() => grab.selectEntered.RemoveListener(OnAgarrado);

    private void OnAgarrado(SelectEnterEventArgs args)
    {
        if (consumido) return;

        // El jugador es quien lo agarró: sube desde su mano hasta el SistemaSalud
        SistemaSalud salud = args.interactorObject.transform.GetComponentInParent<SistemaSalud>();

        if (salud == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("jugador");
            if (jugador == null)
            {
                Debug.LogWarning("[Burrito] No existe ningún objeto con el tag 'jugador'");
                return;
            }

            salud = jugador.GetComponentInChildren<SistemaSalud>();
            if (salud == null)
            {
                Debug.LogWarning("[Burrito] '" + jugador.name + "' tiene el tag, pero no tiene SistemaSalud (ni en sus hijos)");
                return;
            }
        }

        consumido = true;
        salud.CurarCompletamente();   // o salud.Curar(curacion)

        if (sonido != null) sonido.Play();

        // Frena el sube y baja del padre para que no pelee con la mano
        MovimientoVertical mov = GetComponentInParent<MovimientoVertical>();
        if (mov != null) mov.enabled = false;

        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;

        GameObject raiz = mov != null ? mov.gameObject : gameObject;
        Destroy(raiz, tiempoAntesDeDesaparecer);
    }
}