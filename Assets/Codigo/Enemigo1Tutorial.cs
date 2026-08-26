using UnityEngine;

public class Enemigo1Tutorial : MonoBehaviour
{
    public enum EstadoEnemigo
    {
        vigilancia,
        sospecha,
        ataque,
        persecucion,
        huida,
        impactado,
        grito,
        desaparece,
        neutraliza
    }

    public EstadoEnemigo estadoActual = EstadoEnemigo.vigilancia;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (estadoActual)
        {
            case EstadoEnemigo.vigilancia:
                ModoVigilancia();
                break;

            case EstadoEnemigo.sospecha:
                ModoSospecha();
                break;

            case EstadoEnemigo.ataque:
                ModoAtaque();
                break;

            case EstadoEnemigo.persecucion:
                ModoPersecucion();
                break;

            case EstadoEnemigo.huida:
                ModoHuida();
                break;

            case EstadoEnemigo.impactado:
                ModoImpactado();
                break;

            case EstadoEnemigo.grito:
                ModoGrito();
                break;

            case EstadoEnemigo.desaparece:
                ModoDesaparece();
                break;

            case EstadoEnemigo.neutraliza:
                ModoNeutraliza();
                break;

            default:
                break;
        }
    }

    void ModoVigilancia()
    {
        if (jugadorCubierto())
        {
            CambiarEstado(EstadoEnemigo.sospecha);
        }
        else if (VerJugador())
        {
            CambiarEstado(EstadoEnemigo.ataque);
        }
    }

    void ModoSospecha()
    {
        if (VerJugador())
        {
            CambiarEstado(EstadoEnemigo.ataque);
        }
        else if (jugadorLejos())
        {
            CambiarEstado(EstadoEnemigo.vigilancia);
        }
    }

    void ModoAtaque()
    {
        if (jugadorEscapa())
        {
            CambiarEstado(EstadoEnemigo.persecucion);
        }
        else if (enemigoCasiVencido())
        {
            CambiarEstado(EstadoEnemigo.huida); //casi vence al enemigp
        }
        else if (EnemigoVencido())
        {
            CambiarEstado(EstadoEnemigo.desaparece); //vence al enemigo
        }
    }

    void ModoPersecucion()
    {
        if (jugadorEscapa())
        {
            CambiarEstado(EstadoEnemigo.vigilancia);
        }
        else if (EnemigoGana())
        {
            CambiarEstado(EstadoEnemigo.ataque);
        }
    }

    void ModoHuida()
    {
        if (EnemigoVencido())
        {
            CambiarEstado(EstadoEnemigo.desaparece);
        }
    }

    void ModoImpactado()
    {
        if (jugadorEscapa())
        {
            CambiarEstado(EstadoEnemigo.vigilancia);
        }

        if (jugadorEnZona())
        {
            CambiarEstado(EstadoEnemigo.grito);
        }
        else
        {
            CambiarEstado(EstadoEnemigo.vigilancia);
        }
    }

    void ModoGrito()
    {
        AlertaAGuardias(); //alerta a los guardias cuando grita

        if (jugadorEscapa())
        {
            CambiarEstado(EstadoEnemigo.vigilancia);
        }
    }

    void ModoDesaparece()
    {
        Destroy(gameObject); //destruido
    }

    void ModoNeutraliza()
    {
        MoviemientoJugadorDesabilitado(); //para no poder moverse el jugador
    }

    void CambiarEstado(EstadoEnemigo nuevoEstado)
    {
        estadoActual = nuevoEstado;
    }

    public void DeslumbradoPorFlash()
    {
        //solo en modo ataque
        if (estadoActual == EstadoEnemigo.ataque)
        {
            CambiarEstado(EstadoEnemigo.impactado);
        }
    }

    bool jugadorCubierto() => false;
    bool VerJugador() => false;
    bool jugadorLejos() => false;
    bool jugadorEscapa() => false;
    bool enemigoCasiVencido() => false;
    bool EnemigoVencido() => false;
    bool jugadorTryEscape() => false;
    bool EnemigoGana() => false;
    bool jugadorEnZona() => false;

    void AlertaAGuardias() { }
    void MoviemientoJugadorDesabilitado() { }
}