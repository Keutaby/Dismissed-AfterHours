using UnityEngine;

public class MovimientoVertical : MonoBehaviour
{
    [SerializeField] private float altura = 0.2f;      // cuánto sube y baja (metros)
    [SerializeField] private float velocidad = 1.5f;   // más alto = más rápido
    [SerializeField] private float giro = 0f;          // opcional: grados por segundo para que gire

    private Vector3 posicionInicial;

    void Start() => posicionInicial = transform.localPosition;

    void Update()
    {
        float y = Mathf.Sin(Time.time * velocidad) * altura;
        transform.localPosition = posicionInicial + new Vector3(0f, y, 0f);

        if (giro != 0f) transform.Rotate(0f, giro * Time.deltaTime, 0f, Space.World);
    }
}