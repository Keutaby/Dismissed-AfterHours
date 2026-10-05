using UnityEngine;

public class ManejoObjetosEnMano : MonoBehaviour
{
    public Transform manoTransform;

    private FlashMecanica flashScript;
    private AtaqueRegla reglaScript;

    void Start()
    {
        flashScript = GetComponent<FlashMecanica>();
        reglaScript = GetComponent<AtaqueRegla>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SoltarObjetoActual();
        }
    }

    public bool TieneObjetoEnMano()
    {
        return manoTransform != null && manoTransform.childCount > 0;
    }

    public void SoltarObjetoActual()
    {
        if (manoTransform == null || manoTransform.childCount == 0) return;

        for (int i = manoTransform.childCount - 1; i >= 0; i--)
        {
            Transform item = manoTransform.GetChild(i);

            item.SetParent(null);

            item.position = transform.position + transform.forward * 0.8f;

            Collider[] colliders = item.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = true;
            }

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce(transform.forward * 100f);
            }
        }

        if (flashScript != null) flashScript.enabled = false;
        //if (reglaScript != null) reglaScript.DesactivarRegla();

        Debug.Log("Objeto soltado y mano vacía.");
    }
}
