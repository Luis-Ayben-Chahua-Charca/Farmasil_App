namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Línea de una venta (TBL_DETALLE_VENTAS): una por remesa vendida (DEC-036).
/// Incluye producto y lote para la previsualización de DOC.
/// </summary>
public sealed class DetalleVentaDto
{
    public int IdLote { get; init; }

    public string NombreProducto { get; init; } = string.Empty;

    public string NumeroLote { get; init; } = string.Empty;

    public int Cantidad { get; init; }

    public decimal PrecioUnitario { get; init; }

    /// <summary>Subtotal informativo de la línea (cantidad por precio unitario).</summary>
    public decimal Subtotal => Cantidad * PrecioUnitario;
}