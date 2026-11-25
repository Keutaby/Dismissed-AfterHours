
using UnityEngine;
using TMPro;
using System.Collections;

public class ContadorProyectos : MonoBehaviour
{
    public static ContadorProyectos Instance;

    public int proyectosRecolectados = 0;
    public int totalProyectos = 5;

    public TMP_Text textoContador;
    public CanvasGroup grupoCanvas;

    private Coroutine rutinaDesaparicion;

    private void Awake()
    {
        Instance = this;
        grupoCanvas.alpha = 0;
        ActualizarTexto();
    }

    public void AgregarProyecto()
    {
        proyectosRecolectados++;

        ActualizarTexto();
        MostrarContador();

        Debug.Log("Proyectos: " +
            proyectosRecolectados + "/" + totalProyectos);

        if (proyectosRecolectados >= totalProyectos)
        {
            Debug.Log("¡Todos los proyectos recolectados!");

            GameManager3_4.Instance.CambiarEstado(GameManager3_4.Estados3_4.TransicionNvl5);

        }
    }

    private void ActualizarTexto()
    {
        if (textoContador != null)
        {
            textoContador.text =
                proyectosRecolectados + " / " + totalProyectos;
        }
    }

    private void MostrarContador()
    {
        if (rutinaDesaparicion != null)
        {
            StopCoroutine(rutinaDesaparicion);
        }

        rutinaDesaparicion = StartCoroutine(DesaparecerContador());
    }

    private IEnumerator DesaparecerContador()
    {
        grupoCanvas.alpha = 1;

        yield return new WaitForSeconds(2f);

        float duracion = 0.5f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            grupoCanvas.alpha = Mathf.Lerp(
                1f, 0f, tiempo / duracion
            );

            yield return null;
        }

        grupoCanvas.alpha = 0;
        rutinaDesaparicion = null;
    }
}
