
using UnityEngine;

public class AudioRecolectables : MonoBehaviour
{
    public static AudioRecolectables Instance;

    [Header("Sonido de recolección")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoRecoleccion;

    private void Awake()
    {
        Instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirSonido()
    {
        if (audioSource != null && sonidoRecoleccion != null)
        {
            audioSource.PlayOneShot(sonidoRecoleccion);
        }
    }
}
