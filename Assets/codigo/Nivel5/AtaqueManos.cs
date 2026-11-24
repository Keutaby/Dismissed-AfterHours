
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

    [Header("Vulnerabilidad")]
    public float tiempoVulnerable = 5f;
    private int ataquesRealizados = 0;
    public bool EstaVulnerable {get; private set;} = false;

    [Header("Daño al Jugador")]
    public float radioImpacto = 9f;
    public int danoJugador = 25;

    [Header("Indicador de impacto")]
    public GameObject indicadorImpacto;
    public float tiempoAdvertencia = 1.5f;


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

            //Indicador de Ataque
            indicadorImpacto.transform.position = new Vector3(puntoElegido.x,
            puntoElegido.y + 0.03f,
            puntoElegido.z);

            indicadorImpacto.SetActive(true);
            yield return new WaitForSeconds(tiempoAdvertencia);
            //Y aqui viene el ataque!! 

            float alturaFinal = puntoElegido.y + alturaImpacto;

            yield return StartCoroutine(
                MoverVertical(alturaFinal, velocidadCaida)
            );

            indicadorImpacto.SetActive(false);

            if (ataquesRealizados < 4)
            {
                DetectarJugador();
            }

            if (ataquesRealizados >= 4)
            {
                EstaVulnerable = true;
                yield return new WaitForSeconds(tiempoVulnerable);
                EstaVulnerable = false;
                ataquesRealizados = 0;
            }
            else
            {
                ataquesRealizados++;
                Debug.Log("Ataque normal número: " + ataquesRealizados);

                yield return new WaitForSeconds(pausaEnImpacto);
            }

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

    void DetectarJugador()
    {
        Collider[] objetosDetectados = Physics.OverlapSphere(transform.position,
        radioImpacto
        );

        foreach (Collider objeto in objetosDetectados)
        {
            SistemaSalud salud = objeto.GetComponentInParent<SistemaSalud>();

            if(salud != null)
            {
                salud.RecibirDano(danoJugador);
                Debug.Log("A no manches, recibiste daño!!");
                return;
            }
        }
    }
}
