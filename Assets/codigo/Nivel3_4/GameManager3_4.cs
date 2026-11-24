using UnityEngine;
using System;

public class GameManager3_4 : MonoBehaviour
{
    public enum Estados3_4
    {
        //Nivel 3
        EsperandoInicio,

        DialogoProfesorIntro,
        RetiradaProfesorIntro,
        BusquedaGameplayNvl3,

        //Nivel 4
        TransicionNvl4,
        GameplayNvl4,

        MuerteJugador,
        GanadorNvl4,
        TransicionNvl5
    }

    public static GameManager3_4 Instance;
    public Estados3_4 State {get; private set;}
    public event Action<Estados3_4> OnGameStateChanged;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        State = Estados3_4.EsperandoInicio;
    }

    void Start()
    {
        Debug.Log("Estado Actual: " + State);
        CambiarEstado(Estados3_4.DialogoProfesorIntro);
    } 

    public void CambiarEstado(Estados3_4 nuevoEstado)
    {
        if(State == nuevoEstado)
        {
            return;
        }

        Debug.Log("Cambio de estado " +State + "a" + nuevoEstado);
        State = nuevoEstado;
        OnGameStateChanged?.Invoke(State);
    }
}
