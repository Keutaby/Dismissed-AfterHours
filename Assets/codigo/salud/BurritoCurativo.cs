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
        consumido = true;

        GameObject jugador = GameObject.FindGameObjectWithTag("jugador");
        SistemaSalud salud = jugador != null ? jugador.GetComponent<SistemaSalud>() : null;
        if (salud != null) salud.Curar(curacion);
        else Debug.LogWarning("[Burrito] No encontré el SistemaSalud del jugador (tag 'jugador')");

        if (sonido != null) sonido.Play();

        // Desaparece de la vista pero deja terminar el sonido
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
        Destroy(gameObject, tiempoAntesDeDesaparecer);
    }
}