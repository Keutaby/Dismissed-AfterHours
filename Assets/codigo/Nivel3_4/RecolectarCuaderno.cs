using UnityEngine;

public class RecolectarCuaderno : MonoBehaviour
{
    private bool recolectado = false; 
    private void OnTriggerEnter(Collider other)
    {
        if(recolectado)
        return;

        if(other.CompareTag("jugador") || other.transform.root.CompareTag("jugador"))
        {
            recolectado = true;
            AudioRecolectables.Instance?.ReproducirSonido();
            ContadorCuadernos.Instance.AgregarCuaderno();
            Destroy(gameObject);
        }
    }

}
