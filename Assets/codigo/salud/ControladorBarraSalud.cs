using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraSalud : MonoBehaviour
{
    [SerializeField] private SistemaSalud sistemaSaludTarget;
    [SerializeField] private Image mascaraSalud;     // la que se va vaciando
    [SerializeField] private Image imagenDestruida;  // la que se va llenando

    void OnEnable()
    {
        if (sistemaSaludTarget != null)
            sistemaSaludTarget.alCambiarSalud += ActualizarBarra;
    }

    void Start()
    {
        if (sistemaSaludTarget != null)
            ActualizarBarra(sistemaSaludTarget.Porcentaje);
    }

    void OnDisable()
    {
        if (sistemaSaludTarget != null)
            sistemaSaludTarget.alCambiarSalud -= ActualizarBarra;
    }

    private void ActualizarBarra(float porcentaje)
    {
        porcentaje = Mathf.Clamp01(porcentaje);
        if (mascaraSalud != null) mascaraSalud.fillAmount = porcentaje;
        if (imagenDestruida != null) imagenDestruida.fillAmount = 1f - porcentaje;
    }
}