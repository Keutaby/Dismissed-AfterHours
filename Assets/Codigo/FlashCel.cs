using UnityEngine;

public class FlashCel : MonoBehaviour
{
    public Transform CameraPoint;
    public float DistanciaFlash = 5f;

    /*// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {

    }

    void UsarFlashCel()
    {
        RaycastHit hit;
        if (Physics.Raycast(CameraPoint.position, CameraPoint.forward, out hit, DistanciaFlash))
        {
            Enemigo1Tutorial fantasma = hit.collider.GetComponent<Enemigo1Tutorial>();
            if (fantasma != null)
            {
                fantasma.DeslumbradoPorFlash();
            }
        }
    }
}
