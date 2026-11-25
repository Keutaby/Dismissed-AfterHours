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

            Debug.Log("Cuaderno Recolectado!!");
            ContadorCuadernos.Instance.AgregarCuaderno();
            Destroy(gameObject);
        }
    }

}
