using System.Collections.Generic;
using UnityEngine;

public class ControladorGeneral : MonoBehaviour
{
    private Dictionary<string, string> datosJuego = new Dictionary<string, string>();

    public void actualizar(string clave, string valor)
    {
        if (datosJuego.ContainsKey(clave))
        {
            datosJuego[clave] = valor;
        }
        else
        {
            datosJuego.Add(clave, valor);
        }

        Debug.Log($"[ControladorGeneral] Actualizado: {clave} = {valor}");
    }

    public string ObtenerDato(string clave)
    {
        if (datosJuego.TryGetValue(clave, out string valor))
        {
            return valor;
        }
        return null;
    }

    public bool TieneObjeto(string nombreObjeto)
    {
        return datosJuego.ContainsKey(nombreObjeto) && datosJuego[nombreObjeto] == "1";
    }
}