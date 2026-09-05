using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMenu : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject lineaSeleccion;
    public AudioSource audioSource;
    public AudioClip sonidoSeleccion;

    public void OnSelect(BaseEventData eventData)
    {
        lineaSeleccion.SetActive(true);

        if (audioSource != null && sonidoSeleccion != null)
        {
            audioSource.PlayOneShot(sonidoSeleccion);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        lineaSeleccion.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Si el cursor salió del botón, quitamos la selección
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}