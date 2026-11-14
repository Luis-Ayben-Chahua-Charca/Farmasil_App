namespace FarmasilApp.Modelo.Comun;

/// <summary>
/// Estado de una remesa (VL-03, TBL_LOTES.estado_lote). El literal almacenado es 'Bloqueado por devolucion' (DEC-031).
/// Sus transiciones las gobierna la regla única del estado de la remesa (DEC-034).
/// </summary>
public enum EstadoLote
{
    Disponible,
    BloqueadoPorDevolucion,
    Agotado,
}
