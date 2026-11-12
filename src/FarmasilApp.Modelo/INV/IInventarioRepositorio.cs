using FarmasilApp.Modelo.INV;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Operaciones de persistencia necesarias para la consulta de Inventario.
/// </summary>
public interface IInventarioRepositorio
{
    IReadOnlyList<ProductoInventario> Consultar(
        string? busqueda,
        int? idProveedor,
        string? accionTerapeutica);

    IReadOnlyList<int> ObtenerProveedores();

    IReadOnlyList<string> ObtenerAccionesTerapeuticas();
}
