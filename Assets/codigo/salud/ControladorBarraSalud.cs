using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraSalud : MonoBehaviour
{
    public SistemaSalud sistemaSaludTarget;
    private Slider barra;

    void Awake()
    {
        barra = GetComponent<Slider>();
        barra.minValue = 0f;
        barra.maxValue = 1f;
    }

    void Start()
    {
        if (barra != null)
        {
            barra.value = 1f;
        }
    }

    void OnEnable()
    {
        if (sistemaSaludTarget != null)
            sistemaSaludTarget.alCambiarSalud += ActualizarBarra;
    }

    void OnDisable()
    {
        if (sistemaSaludTarget != null)
            sistemaSaludTarget.alCambiarSalud -= ActualizarBarra;
    }

    private void ActualizarBarra(float porcentaje)
    {
        if (barra != null)
            barra.value = porcentaje;
    }
}