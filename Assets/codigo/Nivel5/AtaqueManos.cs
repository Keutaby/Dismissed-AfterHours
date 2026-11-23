using UnityEngine;
using System.Collections;
using UnityEngine.Scripting.APIUpdating;

public class AtaqueManos : MonoBehaviour
{
    [Header("Puntos de Ataque")]
    public Transform[] puntosAtaque;

    [Header("Movimiento")]
    public float velocidad = 5f;
    public float alturaImpacto = 1f;

    [Header("Tiempos")]
    public float esperaAntesAtaque = 2f;
    public float pausaEnImpacto = 1f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
        StartCoroutine(CicloAtaque());
    }

    IEnumerator CicloAtaque()
    {
        while (true)
        {
            yield return new WaitForSeconds(esperaAntesAtaque);
            int indice = Random.Range(0, puntosAtaque.Length);

            Vector3 destino = puntosAtaque[indice].position + Vector3.up * alturaImpacto;


            yield return StartCoroutine(MoverA(destino));
            yield return new WaitForSeconds(pausaEnImpacto);
            yield return StartCoroutine(MoverA(posicionInicial));
        }
    }

    IEnumerator MoverA(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

    transform.position = destino;
    }
}
