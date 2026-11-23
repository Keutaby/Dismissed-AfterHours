using UnityEngine;
using System;
using System.Collections;

public class GameManager5 : MonoBehaviour
{

    public enum EstadosNvl5
    {
        SinIniciar,
        IntroJugador,
        IntroRector,
        PrepararCombate,
        Combate,
        Ganador,
        Perdedor
    }
    public static GameManager5 Instance; // llamo a otros codigos
    public EstadosNvl5 State;
    public EstadosNvl5 newState;
    public static event Action<EstadosNvl5> OnGameStateChanged;


    //----------------Separador jeje porque después me pierdo
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        State = EstadosNvl5.SinIniciar;
    }

    void Start()
    {
        newState = EstadosNvl5.IntroJugador;
        StartCoroutine(PruebaSecuenciaEstados());
    }

    public void CambiarEstado(EstadosNvl5 estadoSocilicitado)
    {
        newState = estadoSocilicitado;
    }

    IEnumerator PruebaSecuenciaEstados()
    {
        yield return new WaitForSeconds(6f);
        CambiarEstado(EstadosNvl5.IntroRector);

        yield return new WaitForSeconds(3f);
        CambiarEstado(EstadosNvl5.PrepararCombate);

        yield return new WaitForSeconds(3f);
        CambiarEstado(EstadosNvl5.Combate);

    }

    void Update()
    {
        if(State != newState)
        {
            State = newState; 
            OnGameStateChanged?.Invoke(State);

            switch (State)
            {
                case EstadosNvl5.IntroJugador:
                Debug.Log("Entra el jugador por la puerta");
                break;

                case EstadosNvl5.IntroRector:
                Debug.Log("Entra el mero mero camotero");
                break;

                case EstadosNvl5.PrepararCombate:
                Debug.Log("Agarra la macana hijo, te van a partir en pedacitos");
                break;

                case EstadosNvl5.Combate:
                Debug.Log("Pelea! Pelea!! le apuesto al grandote!");
                break;

                case EstadosNvl5.Ganador:
                Debug.Log("A no manches, si le ganaste, Nono , no me toques, quiero vivir");
                break;

                case EstadosNvl5.Perdedor:
                Debug.Log("No me toques, igual me caias mal, que bueno que te apachurraron");
                break;
            }
        }
    }
}
