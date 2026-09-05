using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraSalud : MonoBehaviour
{
    [Header("Referencias del Sistema de Salud")]
    public SistemaSalud sistemaSaludTarget;

    [Header("Referencias de UI")]
    [Tooltip("Arrastra aquí el GameObject 'Health_Mask' que tiene el componente Image en modo Filled.")]
    public Image mascaraSalud;

    void Awake()
    {
        // Si no asignaste la máscara desde el inspector, intenta buscarla en este objeto
        if (mascaraSalud == null)
        {
            mascaraSalud = GetComponent<Image>();
        }
    }

    void Start()
    {
        // Inicializamos la máscara llena al 100% (1.0f)
        if (mascaraSalud != null)
        {
            mascaraSalud.fillAmount = 1f;
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
        if (mascaraSalud != null)
        {
            // Ajustamos el nivel de la máscara según el porcentaje de salud (entre 0 y 1)
            mascaraSalud.fillAmount = Mathf.Clamp01(porcentaje);
        }
    }
}