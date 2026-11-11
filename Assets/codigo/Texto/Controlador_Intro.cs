using System.Collections;
using UnityEngine;

public class Controlador_Intro : MonoBehaviour
{
    public CanvasGroup fondoNegro;
    public CanvasGroup textoDespierta;
    public CanvasGroup textoNoDeberias;

    private bool yaIniciada;

    void Start()
    {
        // Todo invisible hasta que el tutorial termine, para no tapar las imágenes
        Ocultar(fondoNegro);
        Ocultar(textoDespierta);
        Ocultar(textoNoDeberias);
    }

    // Lo llama TutorialPaneles al terminar
    public void IniciarIntro()
    {
        Debug.Log("[Intro] IniciarIntro llamado");
        if (yaIniciada) return;
        yaIniciada = true;
        StartCoroutine(PlayIntro());
    }

    [ContextMenu("Probar intro")]
    private void Probar() => IniciarIntro();

    private void Ocultar(CanvasGroup g)
    {
        if (g == null) return;
        g.alpha = 0f;
        g.blocksRaycasts = false;   // un CanvasGroup invisible no debe bloquear el rayo
        g.interactable = false;
    }

    IEnumerator PlayIntro()
    {
        yield return Fade(fondoNegro, 0f, 1f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        yield return Fade(textoDespierta, 0f, 1f, 0.7f);
        yield return new WaitForSeconds(1.0f);

        yield return Fade(textoDespierta, 1f, 0f, 0.5f);
        yield return new WaitForSeconds(0.3f);

        yield return Fade(textoNoDeberias, 0f, 1f, 0.7f);
        yield return new WaitForSeconds(1.0f);

        yield return Fade(textoNoDeberias, 1f, 0f, 0.5f);
        yield return new WaitForSeconds(0.3f);

        yield return Fade(fondoNegro, 1f, 0f, 3f);
    }

    IEnumerator Fade(CanvasGroup group, float start, float end, float duration)
    {
        float elapsed = 0f;
        group.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        group.alpha = end;
    }
}