namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Producto mostrado por la consulta de Inventario (ESP-0006).
/// El stock físico se obtiene de VW_STOCK_FISICO.
/// </summary>
public sealed class ProductoInventario
{
    public int IdProducto { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string AccionTerapeutica { get; init; } = string.Empty;

    public decimal PrecioVenta { get; init; }

    public string EstadoProducto { get; init; } = string.Empty;

    public int StockFisico { get; init; }
}
