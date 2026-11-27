
using UnityEngine;
using TMPro;
using System.Collections;

public class PantallaObjetivos : MonoBehaviour
{
    [Header("Objetivo Nivel 3")]
    [SerializeField] private GameObject canvasObjetivoTareas;
    [SerializeField] private TMP_Text textoObjetivoTareas;
    [SerializeField] private CanvasGroup grupoTareas;

    [Header("Objetivo Nivel 4")]
    [SerializeField] private GameObject canvasObjetivoProyecto;
    [SerializeField] private TMP_Text textoObjetivoProyecto;
    [SerializeField] private CanvasGroup grupoProyecto;

    [Header("Duracion")]
    [SerializeField] private float duracionMensaje = 4f;
    [SerializeField] private float duracionDesvanecimiento = 0.5f;

    private Coroutine mensajeActual;

    private void Start()
    {
        grupoTareas.alpha = 0;
        grupoTareas.interactable = false;
        grupoTareas.blocksRaycasts = false;

        grupoProyecto.alpha = 0;
        grupoProyecto.interactable = false;
        grupoProyecto.blocksRaycasts = false;

        canvasObjetivoTareas.SetActive(false);
        canvasObjetivoProyecto.SetActive(false);

        //coneccion al game manager
        if (GameManager3_4.Instance != null)
        {
            Debug.Log("¡PantallaObjetivos encontró al GameManager!");

            GameManager3_4.Instance.OnGameStateChanged += MostrarObjetivo;
        }
        else
        {
            Debug.LogError("No se encontró GameManager3_4");
        }
    }

    private void OnDisable()
    {
        if (GameManager3_4.Instance != null)
        {
            GameManager3_4.Instance.OnGameStateChanged -= MostrarObjetivo;
        }
    }

    private void MostrarObjetivo(GameManager3_4.Estados3_4 estado)
    {

        if (estado == GameManager3_4.Estados3_4.BusquedaGameplayNvl3)
        {
            MostrarMensaje(
                canvasObjetivoTareas,
                textoObjetivoTareas,
                grupoTareas,
                "Objetivo:\nBusca 3 cuadernos."
            );
        }

        if (estado == GameManager3_4.Estados3_4.GameplayNvl4)
        {
            MostrarMensaje(
                canvasObjetivoProyecto,
                textoObjetivoProyecto,
                grupoProyecto,
                "Nuevo objetivo\nBusca 5 proyectos."
            );
        }
    }

    private void MostrarMensaje(
        GameObject canvas,
        TMP_Text texto,
        CanvasGroup grupo,
        string mensaje)
    {
        if (mensajeActual != null)
        {
            StopCoroutine(mensajeActual);
        }

        canvasObjetivoTareas.SetActive(false);
        canvasObjetivoProyecto.SetActive(false);

        grupoTareas.alpha = 0;
        grupoProyecto.alpha = 0;

        mensajeActual = StartCoroutine(
            MostrarTemporalmente(canvas, texto, grupo, mensaje)
        );
    }

    private IEnumerator MostrarTemporalmente(
        GameObject canvas,
        TMP_Text texto,
        CanvasGroup grupo,
        string mensaje)
    {
        texto.text = mensaje;
        canvas.SetActive(true);

        // Aparecer suavemente
        yield return StartCoroutine(
            CambiarAlpha(grupo, 0, 1)
        );

        yield return new WaitForSeconds(duracionMensaje);

        yield return StartCoroutine(
            CambiarAlpha(grupo, 1, 0)
        );

        canvas.SetActive(false);
        mensajeActual = null;
    }

    private IEnumerator CambiarAlpha(
        CanvasGroup grupo,
        float inicio,
        float final)
    {
        float tiempo = 0;

        grupo.alpha = inicio;

        while (tiempo < duracionDesvanecimiento)
        {
            tiempo += Time.deltaTime;

            grupo.alpha = Mathf.Lerp(
                inicio,
                final,
                tiempo / duracionDesvanecimiento
            );

            yield return null;
        }

        grupo.alpha = final;
    }
}
