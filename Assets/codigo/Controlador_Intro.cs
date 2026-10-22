using UnityEngine;
using System.Collections;

public class Controlador_Intro : MonoBehaviour
{
    public CanvasGroup fondoNegro;
    public CanvasGroup textoDespierta;
    public CanvasGroup textoNoDeberias;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fondoNegro.alpha = 1f;
        textoDespierta.alpha = 0f;
        textoNoDeberias.alpha = 0f;

        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.5f);
        
        yield return Fade(textoDespierta, 0f, 1f, 0.7f);
        yield return new WaitForSeconds(1.0f);//Inicio

        yield return Fade(textoDespierta, 1f, 0f, 0.5f);
        yield return new WaitForSeconds (0.3f);//Fin

        yield return Fade(textoNoDeberias, 0f , 1f, 0.7f);
        yield return new WaitForSeconds (1.0f);//Inicio?? Sip, inicio

        yield return Fade(textoNoDeberias, 1f, 0f, 0.5f);
        yield return new WaitForSeconds (0.3f); //fin?

        yield return Fade (fondoNegro, 1f , 0f, 3f);//Fuera fondo negro wiiiii! 
    }

    IEnumerator Fade(
        CanvasGroup group,
        float start,
        float end,
        float duration)
    {
        float elapsed = 0f;
        group.alpha = start;

        while (elapsed <duration)
        {
            elapsed += Time.deltaTime;

            group.alpha = Mathf.Lerp(start, end, elapsed / duration);

            yield return null;
        }

        group.alpha = end;
    }
}
