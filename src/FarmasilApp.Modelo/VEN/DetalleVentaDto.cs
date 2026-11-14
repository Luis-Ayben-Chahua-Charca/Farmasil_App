namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Línea de una venta (TBL_DETALLE_VENTAS): una por remesa vendida (DEC-036).
/// </summary>
public sealed class DetalleVentaDto
{
    public int IdLote { get; init; }

    public int Cantidad { get; init; }

    public decimal PrecioUnitario { get; init; }
}
