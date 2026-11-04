using System.Collections;
using UnityEngine;

public class PuertaDeslizante : MonoBehaviour
{
    [SerializeField] private Vector3 desplazamiento = new Vector3(0, 3f, 0); // cuánto se mueve al abrir
    [SerializeField] private float duracion = 2f;
    [SerializeField] private AudioSource sonido;

    private bool abierta;

    public void Abrir()
    {
        if (abierta) return;
        abierta = true;
        if (sonido != null) sonido.Play();
        StartCoroutine(Mover());
    }

    private IEnumerator Mover()
    {
        Vector3 inicio = transform.position;
        Vector3 fin = inicio + desplazamiento;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            transform.position = Vector3.Lerp(inicio, fin, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }
}