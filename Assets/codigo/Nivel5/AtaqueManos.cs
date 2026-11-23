
using UnityEngine;
using System.Collections;

public class AtaqueManos : MonoBehaviour
{
    [Header("Puntos de Ataque")]
    public Transform[] puntosAtaque;

    [Header("Velocidades")]
    public float velocidadHorizontal = 8f;
    public float velocidadCaida = 25f;
    public float velocidadSubida = 8f;
    public float alturaImpacto = 1f;

    [Header("Tiempos")]
    public float esperaAntesAtaque = 2f;
    public float pausaEnImpacto = 1f;

    private float alturaInicial;

    void Start()
    {
        alturaInicial = transform.position.y;

        StartCoroutine(CicloAtaque());
    }

    IEnumerator CicloAtaque()
    {
        // Esperar hasta que comience el combate
        while (GameManager5.Instance.State != GameManager5.EstadosNvl5.Combate)
        {
            yield return null;
        }

        while (true)
        {
            yield return new WaitForSeconds(esperaAntesAtaque);

            if (puntosAtaque.Length == 0)
            {
                yield break;
            }

            int indice = Random.Range(0, puntosAtaque.Length);

            Vector3 puntoElegido = puntosAtaque[indice].position;

            Vector3 destinoHorizontal = new Vector3(
                puntoElegido.x,
                alturaInicial,
                puntoElegido.z
            );

            yield return StartCoroutine(MoverHorizontal(destinoHorizontal));

            float alturaFinal = puntoElegido.y + alturaImpacto;

            yield return StartCoroutine(
                MoverVertical(alturaFinal, velocidadCaida)
            );


            yield return new WaitForSeconds(pausaEnImpacto);


            yield return StartCoroutine(
                MoverVertical(alturaInicial, velocidadSubida)
            );
        }
    }

    IEnumerator MoverHorizontal(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadHorizontal * Time.deltaTime
            );

            yield return null;
        }

        transform.position = destino;
    }

    IEnumerator MoverVertical(float destinoY, float velocidad)
    {
        while (Mathf.Abs(transform.position.y - destinoY) > 0.05f)
        {
            float nuevaY = Mathf.MoveTowards(
                transform.position.y,
                destinoY,
                velocidad * Time.deltaTime
            );

            transform.position = new Vector3(
                transform.position.x,
                nuevaY,
                transform.position.z
            );

            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            destinoY,
            transform.position.z
        );
    }
}
