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

    void Update()
    {
        if(!tieneRegla)
        {
            ComprobarRecogerRegla();
            return;
        }

        if(Input.GetMouseButtonDown(0) && Time.time >= tiempoProximoAtaque)
        {
            AtacarConRegla();
            tiempoProximoAtaque = Time.time + tiempoEntreAtaques;
        }
    }

    void ComprobarRecogerRegla()
    {
        Ray rayo = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(rayo, out RaycastHit hit, 3f))
        {
            if(hit.collider.CompareTag("Interactuable") || hit.collider.gameObject.name.ToLower().Contains("regla"))
            {
                //Show prompt "[E] Recoger Regla" via your UI manager

                if(Input.GetKeyDown(KeyCode.E))
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

        if(manoTransform != null)
        {
            reglaEnMano.transform.SetParent(manoTransform);
            reglaEnMano.transform.localPosition = Vector3.zero;
            reglaEnMano.transform.localRotation = Quaternion.identity;
        }

        Collider col = reglaEnMano.GetComponent<Collider>();
        if(col != null) col.enabled = false;

        Debug.Log("[AtaqueRegla] ¡Regla equipada! Lista para atacar.");
    }

    void AtacarConRegla()
    {
        Debug.Log("[AtaqueRegla] ¡Ataque con regla!");

        Ray rayo = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(rayo, out RaycastHit hit, alcanceAtaque, capaEnemigo))
        {
            Enemigo1Tutorial fantasma = hit.collider.GetComponentInParent<Enemigo1Tutorial>();
            
            if(fantasma != null)
            {
                Debug.Log("[AtaqueRegla] ¡Golpaste al fantasma con la regla!");

                SistemaSalud salud = fantasma.GetComponent<SistemaSalud>();
                if(salud != null)
                {
                    salud.RecibirDano(danoRegla);
                }
            }
        }
    }
}