using UnityEngine;

public enum TipoInteraccion
{
    obtenible,   //puedes recoger
    examinable,  //inspect/read
    activable    //switches, doors, buttons
}

public interface InteractuableComportamiento
{
    TipoInteraccion tipo { get; set; }
    string nombre { get; set; }

    void colocar_en(Transform ubicacion);
    void arrojar(float fuerza);
    void soltar();
}