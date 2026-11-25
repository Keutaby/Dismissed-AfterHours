using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

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

    [Header("Generadores")]
    public GeneradorCuadernos generadorCuadernos;

    [Header("EnemigosNivel3")]
    public GameObject[] enemigosNivel3;

    [Header("EnemigosNivel4")]
    public GameObject[] enemigosNivel4;

    [Header("IntroducciónTemporalblabla")]
    public float duracionIntro = 5f;

    [Header("Transicion Nivel 4")]
    public float duracionTransicion = 3f;
    

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        State = Estados3_4.EsperandoInicio;

        if(enemigosNivel4 != null)
        {
            foreach(GameObject enemigo in enemigosNivel4)
            {
            if(enemigo != null)
                {
                    enemigo.SetActive(false);
                }
             }

        }

    }

    void Start()
    {
        Debug.Log("Estado Actual: " + State);
        CambiarEstado(Estados3_4.DialogoProfesorIntro);
        StartCoroutine(IniciarNivel3());
    } 

    private IEnumerator IniciarNivel3()
    {
        Debug.Log("El profesor está presentando el nivel...");

        yield return new WaitForSeconds(duracionIntro);

        CambiarEstado(Estados3_4.RetiradaProfesorIntro);

        Debug.Log("El profesor se está retirando...");

        yield return new WaitForSeconds(2f);

        CambiarEstado(Estados3_4.BusquedaGameplayNvl3);

        Debug.Log("¡Comienza la búsqueda de cuadernos!");
    }

    private IEnumerator TransicionNivel4()
    {
        Debug.Log("Nivel 3 completado, dale nivel 4");
        yield return new WaitForSeconds(duracionTransicion);
        CambiarEstado(Estados3_4.GameplayNvl4);
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

        if(State == Estados3_4.BusquedaGameplayNvl3)
        {
            if(generadorCuadernos != null)
            {
                generadorCuadernos.GenerarCuadernos();
            } else
            {
                Debug.LogError("No se asigno generador");
            }
        }

        if(State == Estados3_4.TransicionNvl4)
        {
            StartCoroutine(TransicionNivel4());
        }

        if (State == Estados3_4.GameplayNvl4)
        {
           if(enemigosNivel3 != null)
            {
                foreach (GameObject enemigo in enemigosNivel3)
                {
                    if(enemigo != null)
                    {
                        Destroy(enemigo);
                    }
                }
            }

            if(enemigosNivel4 != null)
            {
                foreach (GameObject enemigo in enemigosNivel4)
                {
                    if(enemigo != null)
                    {
                        enemigo.SetActive(true);
                    }
                }
            }

            Debug.Log("Los muertos vivientes ya recorren el nivel 4");
            Debug.Log("Comienza el gameplay! NIVEL 4 #########");
        }
    }
}
