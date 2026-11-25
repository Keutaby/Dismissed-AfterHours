using UnityEngine;

[DefaultExecutionOrder(-1000)]   // corre antes que los demás scripts del objeto
public class OmitirSiReinicio : MonoBehaviour
{
    void Awake()
    {
        if (GestorMuerte.ReinicioPorMuerte)
            gameObject.SetActive(false);
    }
}