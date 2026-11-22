namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Operaciones de persistencia del módulo de Inventario.
/// </summary>
public interface IInventarioRepositorio
{
    IReadOnlyList<ProductoInventario> Consultar(
        string? busqueda,
        int? idProveedor,
        string? accionTerapeutica);

    IReadOnlyList<int> ObtenerProveedores();

    IReadOnlyList<string> ObtenerAccionesTerapeuticas();

    /// <summary>
    /// ESP-0005. Registra el producto como Disponible y devuelve
    /// el identificador generado por SQL Server.
    /// </summary>
    int Crear(ProductoEscritura producto);

    /// <summary>
    /// ESP-0007. Actualiza los datos editables del producto.
    /// El identificador permanece intacto.
    /// </summary>
    void Actualizar(
        int idProducto,
        ProductoEscritura producto);

    /// <summary>
    /// ESP-0008. Baja condicional según DEC-009 y DEC-029.
    /// </summary>
    ResultadoBajaProducto DarDeBaja(int idProducto);
}