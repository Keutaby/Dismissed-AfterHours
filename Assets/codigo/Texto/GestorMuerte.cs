using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SistemaSalud))]
public class GestorMuerte : MonoBehaviour
{
    [Header("UI de Muerte")]
    [SerializeField] private GameObject canvasMuerte; // Arrastra aquí tu CanvasMuerte

    private SistemaSalud sistemaSalud;
    private bool yaMuerto = false;

    void Awake()
    {
        sistemaSalud = GetComponent<SistemaSalud>();
    }

    void OnEnable()
    {
        sistemaSalud.alMorir += MostrarPantallaMuerte;
    }

    void OnDisable()
    {
        sistemaSalud.alMorir -= MostrarPantallaMuerte;
    }

    private void MostrarPantallaMuerte()
    {
        if (yaMuerto) return;
        yaMuerto = true;

        // 1. Activamos el Canvas con el mensaje
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(true);
        }

        // 2. Opcional: Pausar el tiempo del juego (los fantasmas se detienen)
        // Time.timeScale = 0f; 

        // 3. Opcional: Si tienes un AudioSource para el sonido de muerte, reprodúcelo aquí
        // if (sonidoMuerte != null) sonidoMuerte.Play();
    }

    // Esta función la llamará el BOTÓN desde el Inspector
    public void BotonReiniciar()
    {
        // Si pausaste el tiempo, debes despausarlo antes de recargar
        // Time.timeScale = 1f; 

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}