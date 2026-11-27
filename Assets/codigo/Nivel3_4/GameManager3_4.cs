using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

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
    public GeneradorProyectos generadorProyectos;

    [Header("EnemigosNivel3")]
    public GameObject[] enemigosNivel3;

    [Header("EnemigosNivel4")]
    public GameObject[] enemigosNivel4;

    [Header("Introduccion Profesor")]
    public AudioSource audioProfesor;
    public Animator animatorProfesor;
    public GameObject profesor;

    public float duracionAnimacionRetirada = 3f;

    [Header("Transicion Nivel 4")]
    public float duracionTransicion = 3f;
    [Header("Audio Transición Nivel 4")]
    public AudioSource audioTransicionNivel4;
    public ControladorProfesor controladorProfesor;
    

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
        CambiarEstado(Estados3_4.DialogoProfesorIntro);
        StartCoroutine(IniciarNivel3());
    } 

    private IEnumerator IniciarNivel3()
    {

        if (audioProfesor != null)
        {
            audioProfesor.Play();

            yield return new WaitWhile(() => audioProfesor.isPlaying);
        }

        CambiarEstado(Estados3_4.RetiradaProfesorIntro);

        if (animatorProfesor != null)
        {
            animatorProfesor.Play("SalidaIntroMaestro");
        }


        yield return new WaitForSeconds(duracionAnimacionRetirada);

        if (profesor != null)
        {
            profesor.SetActive(false);
        }

        CambiarEstado(Estados3_4.BusquedaGameplayNvl3);

    }


    private IEnumerator TransicionNivel4()
    {

        // 1. Reproducir audio de transición
        if (audioTransicionNivel4 != null)
        {
            audioTransicionNivel4.Play();

            // Esperar hasta que termine el audio
            yield return new WaitWhile(() => audioTransicionNivel4.isPlaying);
        }


        // 3. Iniciar Gameplay del nivel 4
        CambiarEstado(Estados3_4.GameplayNvl4);
    }


    public void CambiarEstado(Estados3_4 nuevoEstado)
    {
        if(State == nuevoEstado)
        {
            return;
        }
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

            if (controladorProfesor != null)
            {
                controladorProfesor.IniciarAcecho();
            }
            else
            {
                Debug.LogError("No se asignóel controlador profesro");
            }

            if (generadorProyectos != null)
            {
                generadorProyectos.GenerarProyectos();

            }
            else
            {
                Debug.LogError ("Nose asigno el generador de proyectinis");
            }

        }

        if (State == Estados3_4.TransicionNvl5)
        {
            SceneManager.LoadScene("Nivel5");
        }


    }
}
