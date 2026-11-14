namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Remesa vendible que sugiere <see cref="ILotes.SugerirFefo"/>. Trae el nombre y el precio del medicamento:
/// así Ventas no depende de Inventario (VL-04).
/// </summary>
public sealed class RemesaVendibleDto
{
    public int IdLote { get; init; }

    public int IdProducto { get; init; }

    public string NombreMedicamento { get; init; } = string.Empty;

    public decimal PrecioVenta { get; init; }

    public string NumeroLote { get; init; } = string.Empty;

    public DateOnly FechaVencimiento { get; init; }

    public int StockActual { get; init; }
}
