namespace FarmasilApp.Modelo.Comun;

/// <summary>
/// Tipo de una restricción de venta (VL-03, TBL_RESTRICCIONES_VENTA.tipo_restriccion).
/// Una restricción bloqueante impide agregar la línea; una informativa solo se muestra (ESP-0001).
/// </summary>
public enum TipoRestriccion
{
    Informativa,
    Bloqueante,
}
