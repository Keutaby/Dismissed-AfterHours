using UnityEngine;

public class AtaqueRegla : MonoBehaviour
{
    public float alcanceAtaque = 2.5f;
    public int danoRegla = 25;
    public float tiempoEntreAtaques = 0.5f;


    public Transform manoTransform;
    public LayerMask capaEnemigo;

    private GameObject reglaEnMano;
    private bool tieneRegla = false;
    private float tiempoProximoAtaque = 0f;
    private ManejoObjetosEnMano manejoMano;

    void Start()
    {
        manejoMano = GetComponent<ManejoMano>();
    }

    void Update()
    {
        if (!tieneRegla)
        {
            ComprobarRecogerRegla();
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= tiempoProximoAtaque)
        {
            AtacarConRegla();
            tiempoProximoAtaque = Time.time + tiempoEntreAtaques;
        }
    }

    void ComprobarRecogerRegla()
    {
        if (manejoMano != null && manejoMano.TieneObjetoEnMano()) return;

        Ray rayo = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(rayo, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("interactuable") || hit.collider.gameObject.name.ToLower().Contains("regla"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EquiparRegla(hit.collider.gameObject);
                }
            }
        }
    }

    void EquiparRegla(GameObject regla)
    {
        tieneRegla = true;
        reglaEnMano = regla;

        if (manoTransform != null)
        {
            reglaEnMano.transform.SetParent(manoTransform);
            reglaEnMano.transform.localPosition = Vector3.zero;
            reglaEnMano.transform.localRotation = Quaternion.identity;

            Rigidbody rulerRb = reglaEnMano.GetComponent<Rigidbody>();
            if (rulerRb != null) rulerRb.isKinematic = true;
        }

        Collider col = reglaEnMano.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("Regla equipada.");
    }

    void AtacarConRegla()
    {
        Debug.Log("Atacando con regla...");
        Debug.DrawRay(transform.position, transform.forward * alcanceAtaque, Color.yellow, 1.0f);

        Ray rayo = new Ray(transform.position, transform.forward);

        if (Physics.SphereCast(rayo, 0.4f, out RaycastHit hit, alcanceAtaque, capaEnemigo))
        {
            Debug.Log("Objeto impactado: " + hit.collider.gameObject.name);

            SistemaSalud salud = hit.collider.GetComponentInParent<SistemaSalud>();
            if (salud != null)
            {
                salud.RecibirDano(danoRegla);
                Debug.Log("Daño infligido al fantasma!");
            }
        }
    }
}