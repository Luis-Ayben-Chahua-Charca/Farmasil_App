namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Datos editables de un producto para ESP-0005 y ESP-0007.
/// El identificador y el estado no se capturan manualmente.
/// </summary>
public sealed class ProductoEscritura
{
    public string Nombre { get; init; } = string.Empty;

    public string AccionTerapeutica { get; init; } = string.Empty;

    public decimal PrecioVenta { get; init; }

    public int IdProveedor { get; init; }

    public int StockMinimo { get; init; }
}