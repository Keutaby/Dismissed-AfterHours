using UnityEngine;

public class VidaDirector : MonoBehaviour
{
    public GameObject[] manosVida;
    public int vidaActual;

    void Start()
    {
        vidaActual = manosVida.Length;
        Debug.Log("La vida del rector es: " + vidaActual);
    }

    public void RecibirDanio()
    {
        if(vidaActual <= 0)
        {
            return;
            
        }

        vidaActual--;
        manosVida[vidaActual].SetActive(false);
        Debug.Log("Hijolee, llamen al doitor que falta una mano, ahora le queda " + vidaActual);

        if (vidaActual <= 0)
        {
            GameManager5.Instance.CambiarEstado(GameManager5.EstadosNvl5.Ganador);
        }
    }

    
}
