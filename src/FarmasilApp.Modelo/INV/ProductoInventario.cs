namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Producto mostrado y seleccionado en Inventario.
/// El stock físico se obtiene de VW_STOCK_FISICO.
/// </summary>
public sealed class ProductoInventario
{
    public int IdProducto { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string AccionTerapeutica { get; init; } = string.Empty;

    public decimal PrecioVenta { get; init; }

    public string EstadoProducto { get; init; } = string.Empty;

    public int IdProveedor { get; init; }

    public int StockMinimo { get; init; }

    public int StockFisico { get; init; }
}