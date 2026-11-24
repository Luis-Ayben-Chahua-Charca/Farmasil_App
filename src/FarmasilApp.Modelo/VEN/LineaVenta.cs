namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Línea del detalle de la venta en curso: una por remesa (DEC-036). Todavía no se guarda; el registro es del #99.
/// </summary>
public sealed class LineaVenta
{
    public int IdLote { get; init; }

    public int IdProducto { get; init; }

    public string NombreMedicamento { get; init; } = string.Empty;

    public string NumeroLote { get; init; } = string.Empty;

    public int Cantidad { get; init; }

    /// <summary><c>precio_venta</c> vigente del medicamento cuando se agregó la línea.</summary>
    public decimal PrecioUnitario { get; init; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}
