using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class AnimIntroRector : MonoBehaviour
{
    [Header("Animador del rector")]
    public Animator animador;

    [Header("Aparicion")]
    public float distanciaEmerge = 41.11f;
    public float duracionAparicion = 3f;

    private Vector3 posicionFinal;

    void Start()
    {
        posicionFinal = transform.position;
        StartCoroutine(Aparecer());
    }

    IEnumerator Aparecer()
    {
        Vector3 posicionInicial = posicionFinal;
        posicionInicial.y -= distanciaEmerge;

        transform.position = posicionInicial;

        animador.Play("Armature|Intro_rector", 0, 0f);

        float tiempo = 0f;

        while (tiempo < duracionAparicion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo /duracionAparicion;

            transform.position = Vector3.Lerp(
                posicionInicial,
                posicionFinal, 
                progreso);

                yield return null;
        }

        transform.position = posicionFinal;

        animador.Play("Idle", 0, 0f);

    }
}
