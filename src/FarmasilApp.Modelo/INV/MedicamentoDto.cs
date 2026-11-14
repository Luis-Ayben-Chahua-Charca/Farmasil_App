using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Medicamento del catálogo que ofrece <see cref="IInventario"/> (TBL_PRODUCTOS) a los demás módulos.
/// No incluye el stock: el stock físico y el vendible los exponen Lotes y sus vistas (DEC-056).
/// </summary>
public sealed class MedicamentoDto
{
    public int IdProducto { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string AccionTerapeutica { get; init; } = string.Empty;

    public decimal PrecioVenta { get; init; }

    public EstadoProducto Estado { get; init; }

    /// <summary>Proveedor habitual del medicamento; el de cada remesa puede ser otro (contexto de arquitectura, §5).</summary>
    public int IdProveedor { get; init; }

    public int StockMinimo { get; init; }
}
