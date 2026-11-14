namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Operaciones de persistencia necesarias para la consulta de Proveedores.
/// </summary>
public interface IProveedoresRepositorio
{
    IReadOnlyList<ProveedorConsulta> Consultar(
        string? busqueda,
        string? estado);
}
