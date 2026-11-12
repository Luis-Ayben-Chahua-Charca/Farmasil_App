using FarmasilApp.Modelo.INV;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Servicio de aplicación para ESP-0006.
/// </summary>
public sealed class InventarioServicio
{
    private readonly IInventarioRepositorio _repositorio;

    public InventarioServicio(IInventarioRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
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

    public IReadOnlyList<int> ObtenerProveedores()
    {
        return _repositorio.ObtenerProveedores();
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
