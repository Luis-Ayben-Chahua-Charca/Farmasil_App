namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Datos validados necesarios para registrar una nueva remesa.
/// </summary>
public sealed class NuevaRemesa
{
    public int IdProducto { get; init; }

    public int IdProveedor { get; init; }

    public string NumeroLote { get; init; } = string.Empty;

    public DateOnly FechaVencimiento { get; init; }

    public int StockInicial { get; init; }

    public DateOnly FechaIngreso { get; init; }
}
