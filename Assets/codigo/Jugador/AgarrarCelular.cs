using UnityEngine;
using UnityEngine.InputSystem;

public class AgarrarCelular : MonoBehaviour
{
    public float distanciaInteraccion = 2.5f;
    public Transform manoJugador; // Optional: transform where phone attaches when held

    private Transform jugadorTransform;
    private bool agarrado = false;

    void Start()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("jugador");
        if (jugadorObj != null)
        {
            jugadorTransform = jugadorObj.transform;
        }
    }

    void Update()
    {
        if (agarrado || jugadorTransform == null) return;

        float distancia = Vector3.Distance(transform.position, jugadorTransform.position);

        // Check if player is close enough and presses E (or interaction key)
        Keyboard keyboard = Keyboard.current;
        if (distancia <= distanciaInteraccion && keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            Agarrar();
        }
    }

    void Agarrar()
    {
        agarrado = true;

        // Parent phone to player's hand or disable table object
        if (manoJugador != null)
        {
            transform.SetParent(manoJugador);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            gameObject.SetActive(false); // Hide object if inventory handles it
        }

        Debug.Log("[Tutorial] Celular obtenido. Listo para el lobby.");
    }
}