using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraSalud : MonoBehaviour
{
    [Header("Referencias del Sistema de Salud")]
    public SistemaSalud sistemaSaludTarget;

    [Header("Referencias de UI")]
    [Tooltip("Arrastra aquí la imagen intacta (HealthGood).")]
    public Image mascaraSalud;

    [Tooltip("Arrastra aquí la imagen rota (HealthDestroyed).")]
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

        // La imagen intacta muestra la salud restante (ej: 0.7 de izquierda a derecha)
        if (mascaraSalud != null)
        {
            mascaraSalud.fillAmount = saludNormalizada;
        }

        // La imagen destruida muestra el daño (1 - salud, ej: 0.3 de derecha a izquierda)
        if (imagenDestruida != null)
        {
            imagenDestruida.fillAmount = 1f - saludNormalizada;
        }
    }
}