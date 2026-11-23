namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Operaciones de persistencia del módulo de Proveedores.
/// </summary>
public interface IProveedoresRepositorio
{
    IReadOnlyList<ProveedorConsulta> Consultar(
        string? busqueda,
        string? estado);

    /// <summary>
    /// Proveedores activos, ordenados por identificador. Usado por <see cref="IProveedores"/>.
    /// </summary>
    IReadOnlyList<ProveedorDto> ObtenerActivos();

    /// <summary>
    /// Proveedor por identificador (incluye inactivos), o <c>null</c> si no existe.
    /// </summary>
    ProveedorDto? ObtenerPorId(int idProveedor);

    /// <summary>
    /// Indica si existe un proveedor con ese RUC, opcionalmente excluyendo uno por id (para actualización).
    /// </summary>
    bool ExisteRuc(string ruc, int? excluirIdProveedor = null);

    /// <summary>
    /// Indica si el proveedor tiene órdenes de devolución en estado 'Pendiente'.
    /// </summary>
    bool TieneOrdenesPendientes(int idProveedor);

    /// <summary>
    /// ESP-0037. Registra el proveedor como Activo y devuelve el identificador generado por SQL Server.
    /// </summary>
    int Crear(ProveedorEscritura proveedor);

    /// <summary>
    /// ESP-0039. Actualiza los datos editables del proveedor.
    /// </summary>
    void Actualizar(int idProveedor, ProveedorEscritura proveedor);

    /// <summary>
    /// ESP-0040. Baja condicional según VP-10 y DEC-029.
    /// </summary>
    ResultadoBajaProveedor DarDeBaja(int idProveedor);
}
