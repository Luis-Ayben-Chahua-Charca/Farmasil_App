namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Datos validados necesarios para actualizar una remesa existente.
/// El medicamento, el estado y la fecha de ingreso no se modifican.
/// </summary>
public sealed class ActualizacionRemesa
{
    public int IdProveedor { get; init; }

    public string NumeroLote { get; init; } = string.Empty;

    public DateOnly FechaVencimiento { get; init; }

    public int StockActual { get; init; }
}
