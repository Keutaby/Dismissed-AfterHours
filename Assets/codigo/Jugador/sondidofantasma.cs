using UnityEngine;

public class SondidoFantasma : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoAtaque;
    public AudioClip sonidoMuerte;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playAtaque()
    {
        Debug.Log("playGolpe llamado");
        audioSource.PlayOneShot(sonidoAtaque);
    }
    public void playMuerte()
    {
        Debug.Log("playMuerte llamado");
        audioSource.PlayOneShot(sonidoMuerte);
    }
}
