using UnityEngine;
using TMPro;

public class DetectorInteraccion : MonoBehaviour
{
    public float distanciaRaycast = 6f;
    public TextMeshProUGUI textoPrompt;
    public Transform manoTransform;

    private GameObject objetoAptos;
    private bool tieneTelefono = false;
    private bool esperandoUsoFlash = false;

    void Start()
    {
        // UNCOMMENT THIS LINE TO RESET PERSISTENT MEMORY FOR TESTING:
        PlayerPrefs.DeleteKey("TutorialCompletado"); 

        if (textoPrompt != null)
        {
            textoPrompt.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Raycast for ground phone if we haven't picked it up yet
        if (!tieneTelefono)
        {
            ComprobarMiradaTelefono();
        }

        // 2. Keep flash text visible UNTIL the player presses F or Left-Click
        if (esperandoUsoFlash)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
            {
                CompletarTutorialFlash();
            }
        }
    }

    void ComprobarMiradaTelefono()
    {
        Ray rayo = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(rayo, out RaycastHit hit, distanciaRaycast))
        {
            if (hit.collider.CompareTag("interactuable") || hit.collider.gameObject.name.ToLower().Contains("celular"))
            {
                objetoAptos = hit.collider.gameObject;

                // Show pickup prompt ONLY if tutorial isn't marked completed
                if (PlayerPrefs.GetInt("TutorialCompletado", 0) == 0)
                {
                    MostrarTexto("[E] Recoger Teléfono");
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    RecogerTelefono();
                }
                return;
            }
        }

        if (!esperandoUsoFlash)
        {
            OcultarTexto();
        }
    }

    void RecogerTelefono()
    {
        tieneTelefono = true;

        // Parent the desk phone to the hand transform
        if (objetoAptos != null && manoTransform != null)
        {
            objetoAptos.transform.SetParent(manoTransform);
            objetoAptos.transform.localPosition = Vector3.zero;
            objetoAptos.transform.localRotation = Quaternion.identity;

            Collider col = objetoAptos.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        // Only show flash tutorial if the player hasn't done it before
        if (PlayerPrefs.GetInt("TutorialCompletado", 0) == 0)
        {
            MostrarTexto("[F] / [Clic Izq] Usar Flash");
            esperandoUsoFlash = true;
        }
        else
        {
            OcultarTexto();
        }
    }

    void CompletarTutorialFlash()
    {
        esperandoUsoFlash = false;
        OcultarTexto();

        // Save that the player completed the tutorial so it never shows again on death
        PlayerPrefs.SetInt("TutorialCompletado", 1);
        PlayerPrefs.Save();
    }

    public void MostrarTexto(string mensaje)
    {
        if (textoPrompt != null)
        {
            textoPrompt.text = mensaje;
            textoPrompt.gameObject.SetActive(true);
        }
    }

    public void OcultarTexto()
    {
        if (textoPrompt != null)
        {
            textoPrompt.gameObject.SetActive(false);
        }
    }
}