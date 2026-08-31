using UnityEngine;

public class RecogibleComportamiento : MonoBehaviour, InteractuableComportamiento
{
    public TipoInteraccion tipo { get; set; }
    public string nombre { get; set; }

    [Header("Configuración del Objeto")]
    public string nombre_del_objeto = "Celular";

    private ControladorGeneral controlador;

    void Start()
    {
        tipo = TipoInteraccion.obtenible;
        nombre = nombre_del_objeto;

        controlador = GameObject.FindAnyObjectByType<ControladorGeneral>();
    }

    public void colocar_en(Transform mano)
    {
        if (mano != null)
        {
            // 1. Re-parent to the hand object
            transform.SetParent(mano);

            // 2. Zero out local positions so it snaps to the hand, NOT world coords (-67)
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // 3. Disable physics so it follows the player smoothly
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    public void arrojar(float fuerza) { }
    public void soltar() { }
}