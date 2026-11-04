using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LuzParpadeante : MonoBehaviour
{
    [Header("Tiempo encendida (segundos)")]
    [SerializeField] private float encendidaMin = 0.5f;
    [SerializeField] private float encendidaMax = 3f;

    [Header("Tiempo apagada (segundos)")]
    [SerializeField] private float apagadaMin = 0.05f;
    [SerializeField] private float apagadaMax = 0.3f;

    [Header("Opcional")]
    [SerializeField] private Renderer focoVisual;   // el modelo del foco, para que también se apague
    [SerializeField] private AudioSource sonido;    // zumbido o "clic" al apagarse

    private Light luz;
    private float intensidadOriginal;

    void Awake()
    {
        luz = GetComponent<Light>();
        intensidadOriginal = luz.intensity;
    }

    void OnEnable() => StartCoroutine(Parpadear());

    private IEnumerator Parpadear()
    {
        while (true)
        {
            Establecer(true);
            yield return new WaitForSeconds(Random.Range(encendidaMin, encendidaMax));

            // A veces hace un parpadeo doble, se ve más realista
            int veces = Random.value < 0.4f ? 2 : 1;
            for (int i = 0; i < veces; i++)
            {
                Establecer(false);
                if (sonido != null) sonido.Play();
                yield return new WaitForSeconds(Random.Range(apagadaMin, apagadaMax));
                Establecer(true);
                if (i < veces - 1) yield return new WaitForSeconds(0.08f);
            }
        }
    }

    private void Establecer(bool encendida)
    {
        luz.intensity = encendida ? intensidadOriginal : 0f;

        if (focoVisual != null)
        {
            if (encendida) focoVisual.material.EnableKeyword("_EMISSION");
            else focoVisual.material.DisableKeyword("_EMISSION");
        }
    }
}
