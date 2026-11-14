namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Interfaz provista por Proveedores. La usan Inventario, Lotes, Devoluciones y Comparación de Precios.
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface IProveedores
{
    /// <summary>Proveedores con estado 'Activo', en el orden de su identificador.</summary>
    IReadOnlyList<ProveedorDto> ProveedoresActivos();

    /// <summary>
    /// Proveedor con ese identificador, aunque esté inactivo, o <c>null</c> si no existe.
    /// Sirve para mostrar el nombre del proveedor de un registro ya guardado.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">El identificador no es positivo.</exception>
    ProveedorDto? ObtenerProveedor(int idProveedor);
}
