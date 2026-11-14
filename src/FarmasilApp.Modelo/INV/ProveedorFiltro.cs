namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Opción del filtro de proveedor de la consulta de Inventario (<c>INV-CMB-FILTRO-PROVEEDOR</c>):
/// muestra la razón social y filtra por el identificador del proveedor.
/// </summary>
public sealed class ProveedorFiltro
{
    /// <summary>Opción «Todos»: no filtra por proveedor.</summary>
    public static ProveedorFiltro Todos { get; } = new() { IdProveedor = null, Nombre = "Todos" };

    public int? IdProveedor { get; init; }

    public string Nombre { get; init; } = string.Empty;
}
