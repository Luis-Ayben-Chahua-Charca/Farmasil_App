using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Servicio de aplicación para ESP-0006.
/// </summary>
public sealed class InventarioServicio
{
    private readonly IInventarioRepositorio _repositorio;
    private readonly IProveedores _proveedores;

    public InventarioServicio(
        IInventarioRepositorio repositorio,
        IProveedores proveedores)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));

        _proveedores = proveedores
            ?? throw new ArgumentNullException(nameof(proveedores));
    }

    public IReadOnlyList<ProductoInventario> Consultar(
        string? busqueda,
        int? idProveedor,
        string? accionTerapeutica)
    {
        return _repositorio.Consultar(
            Normalizar(busqueda),
            idProveedor,
            Normalizar(accionTerapeutica));
    }

    /// <summary>
    /// Proveedores de los productos de Inventario para el filtro. Los identificadores salen de la propia tabla de
    /// productos y el nombre, de <see cref="IProveedores"/>: Inventario no lee la tabla de Proveedores (DEC-065).
    /// Si Proveedores no conoce un identificador, se muestra como «Proveedor N».
    /// </summary>
    public IReadOnlyList<ProveedorFiltro> ObtenerProveedores()
    {
        return _repositorio.ObtenerProveedores()
            .Select(idProveedor => new ProveedorFiltro
            {
                IdProveedor = idProveedor,
                Nombre = _proveedores.ObtenerProveedor(idProveedor)?.RazonSocial
                    ?? $"Proveedor {idProveedor}",
            })
            .ToArray();
    }

    public IReadOnlyList<string> ObtenerAccionesTerapeuticas()
    {
        return _repositorio.ObtenerAccionesTerapeuticas();
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
