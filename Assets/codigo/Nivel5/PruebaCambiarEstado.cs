using System;
using UnityEngine;

public class PruebaCambiarEstado : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("aplaste la tecla 2!");
            GameManager5.Instance.CambiarEstado(GameManager5.EstadosNvl5.IntroRector);
        }
    } 
}
