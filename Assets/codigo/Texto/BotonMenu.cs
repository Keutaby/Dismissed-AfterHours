using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMenu : MonoBehaviour, ISelectHandler, IDeselectHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public GameObject textoBlanco;     // letra en blanco (estado normal)
    public GameObject textoNegro;      // letra en negro (estado seleccionado)
    public GameObject lineaSeleccion;  // la línea negra

    public AudioSource audioSource;
    public AudioClip sonidoSeleccion;

    public void OnPointerDown(PointerEventData e) => Debug.Log("DOWN en " + name);
    public void OnPointerClick(PointerEventData e) => Debug.Log("CLICK en " + name);

    void OnEnable() => Aplicar(false);

    private void Aplicar(bool seleccionado)
    {
        if (textoBlanco != null) textoBlanco.SetActive(!seleccionado);
        if (textoNegro != null) textoNegro.SetActive(seleccionado);
        if (lineaSeleccion != null) lineaSeleccion.SetActive(seleccionado);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Aplicar(true);

        if (audioSource != null && sonidoSeleccion != null)
            audioSource.PlayOneShot(sonidoSeleccion);
    }

    public void OnDeselect(BaseEventData eventData) => Aplicar(false);

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }
}