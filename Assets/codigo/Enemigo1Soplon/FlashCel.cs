using UnityEngine;
using UnityEngine.InputSystem;

public class FlashCel : MonoBehaviour
{
    public InputActionProperty botonFlashAction;

    public Transform CameraPoint;
    public float DistanciaFlash = 5f;
    public LayerMask enemyLayer;

    /*// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if(botonFlashAction.action != null && botonFlashAction.action.WasPressedThisFrame()){
            UsarFlashCel();
        }
    }

    void UsarFlashCel(){
        RaycastHit hit;
        if (Physics.Raycast(CameraPoint.position, CameraPoint.forward, out hit, DistanciaFlash))
        {
            Enemigo1Tutorial fantasma = hit.collider.GetComponent<Enemigo1Tutorial>();
                if(fantasma != null){
                    fantasma.DeslumbradoPorFlash();
                }
        }
    }
}
