using UnityEngine;
using UnityEngine.Events;

public class ObjetivoMatarFantasmas : MonoBehaviour
{
    [SerializeField] private SistemaSalud[] fantasmas;   // arrastra aquí el Soplón y el Peleonero
    [SerializeField] private UnityEvent alCompletar;     // qué pasa cuando mueren todos

    private int restantes;

    void Start()
    {
        restantes = fantasmas.Length;

        foreach (SistemaSalud f in fantasmas)
        {
            if (f != null) f.alMorir += UnFantasmaMurio;
            else restantes--;
        }
    }

    void OnDestroy()
    {
        foreach (SistemaSalud f in fantasmas)
            if (f != null) f.alMorir -= UnFantasmaMurio;
    }

    private void UnFantasmaMurio()
    {
        restantes--;
        Debug.Log("[Nivel] Fantasmas restantes: " + restantes);

        if (restantes <= 0) alCompletar?.Invoke();
    }
}