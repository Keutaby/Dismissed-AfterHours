using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraSalud : MonoBehaviour
{
    public SistemaSalud sistemaSaludTarget;

    public Image mascaraSalud;
    public Image imagenDestruida;

    void Awake()
    {
        if (mascaraSalud == null)
        {
            mascaraSalud = GetComponent<Image>();
        }
    }

    void Start()
    {
        ActualizarBarra(1f);
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
        float saludNormalizada = Mathf.Clamp01(porcentaje);

        if (mascaraSalud != null)
        {
            mascaraSalud.fillAmount = saludNormalizada;
        }

        if (imagenDestruida != null)
        {
            imagenDestruida.fillAmount = 1f - saludNormalizada;
        }
    }
}