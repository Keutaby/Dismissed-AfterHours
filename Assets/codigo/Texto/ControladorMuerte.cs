using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SistemaSalud))]
public class ControladorMuerte : MonoBehaviour
{
    [Header("Fondo (opcional)")]
    [SerializeField] private CanvasGroup canvasGroupFondo; // alpha = 1, siempre visible

    [Header("Textos")]
    [SerializeField] private CanvasGroup textoMuerte1;
    [SerializeField] private CanvasGroup textoMuerte2;

    [Header("Configuración")]
    [SerializeField] private float duracionFade = 0.6f;
    [SerializeField] private float tiempoVisible = 1.5f;

    private SistemaSalud sistemaSalud;
    private bool yaMuerto = false;

    void Awake() => sistemaSalud = GetComponent<SistemaSalud>();
    void OnEnable() => sistemaSalud.alMorir += IniciarMuerte;
    void OnDisable() => sistemaSalud.alMorir -= IniciarMuerte;

    private void IniciarMuerte()
    {
        if (yaMuerto) return;
        yaMuerto = true;
        StartCoroutine(SecuenciaMuerte());
    }

    private IEnumerator SecuenciaMuerte()
    {
        // 1. Activar el Canvas y el fondo
        gameObject.SetActive(true); // Asegúrate de que el canvas esté en un objeto aparte o usa una referencia
        if (canvasGroupFondo != null) canvasGroupFondo.alpha = 1f;

        // 2. Ocultar ambos textos
        if (textoMuerte1 != null) textoMuerte1.alpha = 0f;
        if (textoMuerte2 != null) textoMuerte2.alpha = 0f;

        yield return new WaitForSeconds(0.3f);

        // 3. Primer texto
        yield return Fade(textoMuerte1, 0f, 1f, duracionFade);
        yield return new WaitForSeconds(tiempoVisible);
        yield return Fade(textoMuerte1, 1f, 0f, duracionFade);

        yield return new WaitForSeconds(0.3f);

        // 4. Segundo texto
        yield return Fade(textoMuerte2, 0f, 1f, duracionFade);
        yield return new WaitForSeconds(tiempoVisible);

        // 5. Al final, permitir clic en el botón
        // (Asegúrate de que el botón esté fuera de los textos y siempre visible, o que su CanvasGroup esté activo)
    }

    IEnumerator Fade(CanvasGroup g, float start, float end, float duration)
    {
        if (g == null) yield break;
        float elapsed = 0f;
        g.alpha = start;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            g.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        g.alpha = end;
    }

    public void BotonReiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}