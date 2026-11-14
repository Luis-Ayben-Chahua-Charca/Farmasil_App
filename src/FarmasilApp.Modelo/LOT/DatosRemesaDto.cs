using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Datos de una remesa (TBL_LOTES) para los módulos que la consultan sin ser su dueño: Ventas, Devoluciones,
/// Alertas de Vencimiento y Reabastecimiento (DEC-057).
/// </summary>
public sealed class DatosRemesaDto
{
    public int IdLote { get; init; }

    public string NumeroLote { get; init; } = string.Empty;

    public DateOnly FechaVencimiento { get; init; }

    public string NombreMedicamento { get; init; } = string.Empty;

    public int IdProveedor { get; init; }

    public EstadoLote EstadoLote { get; init; }

    public int StockActual { get; init; }
}
