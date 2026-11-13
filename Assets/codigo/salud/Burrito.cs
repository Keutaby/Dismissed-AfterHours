using UnityEngine;

public class Burrito : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SistemaSalud salud = other.GetComponent<SistemaSalud>();

        if (salud != null)
        {
            salud.CurarCompletamente();
            Destroy(gameObject);
        }
    }
}