using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Remesa completa usada por el módulo LOT para consulta, selección y edición.
/// </summary>
public sealed class RemesaConsulta
{
    public int IdLote { get; init; }

    public int IdProducto { get; init; }

    public string NombreMedicamento { get; init; } = string.Empty;

    public string NumeroLote { get; init; } = string.Empty;

    public DateOnly FechaVencimiento { get; init; }

    public int StockActual { get; init; }

    public EstadoLote Estado { get; init; }

    public DateOnly FechaIngreso { get; init; }

    public int IdProveedor { get; init; }
}
