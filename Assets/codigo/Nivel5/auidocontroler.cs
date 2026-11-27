using UnityEngine;

public class auidocontroler : MonoBehaviour

{
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoMuerte;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playGolpe()
    {
        Debug.Log("playGolpe llamado");
        audioSource.PlayOneShot(sonidoGolpe);
    }
    public void playMuerte()
    {
        Debug.Log("playMuerte llamado");
        audioSource.PlayOneShot(sonidoMuerte);
    }
}
