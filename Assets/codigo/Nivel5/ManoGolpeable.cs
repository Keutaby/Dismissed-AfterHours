using UnityEngine;

public class ManoGolpeable : MonoBehaviour
{
    [SerializeField] private AtaqueManos ataque;    // si lo dejas vacío, lo busca en este objeto o sus padres
    [SerializeField] private VidaDirector vida;     // si lo dejas vacío, lo busca en la escena
    [SerializeField] private int golpesPorVentana = 1;   // cuántas manos se pierden en cada ventana de vulnerabilidad
    [SerializeField] private float cooldown = 0.5f;

    private int golpesEnVentana;
    private float siguienteGolpe;

    void Awake()
    {
        if (ataque == null) ataque = GetComponentInParent<AtaqueManos>();
        if (vida == null) vida = FindFirstObjectByType<VidaDirector>();
    }

    void Update()
    {
        // Cuando termina la ventana de vulnerabilidad, se reinicia el conteo
        if (ataque != null && !ataque.EstaVulnerable) golpesEnVentana = 0;
    }

    // Lo llama ReglaT. Devuelve true si el golpe contó.
    public bool RecibirGolpe()
    {
        if (ataque == null || vida == null)
        {
            Debug.LogWarning("[Mano] Falta AtaqueManos o VidaDirector");
            return false;
        }

        if (!ataque.EstaVulnerable)
        {
            Debug.Log("[Mano] Golpeada, pero todavía no está vulnerable");
            return false;
        }

        if (Time.time < siguienteGolpe || golpesEnVentana >= golpesPorVentana) return false;

        siguienteGolpe = Time.time + cooldown;
        golpesEnVentana++;
        vida.RecibirDanio();
        return true;
    }
}