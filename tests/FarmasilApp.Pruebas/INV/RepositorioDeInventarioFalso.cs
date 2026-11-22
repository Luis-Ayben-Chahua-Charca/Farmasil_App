using FarmasilApp.Modelo.INV;

namespace FarmasilApp.Pruebas.INV;

/// <summary>
/// Repositorio de Inventario de prueba.
/// Guarda las operaciones recibidas para verificar al servicio sin usar SQL Server.
/// </summary>
internal sealed class RepositorioDeInventarioFalso : IInventarioRepositorio
{
    public List<int> IdsDeProveedor { get; } = new();

    public List<string> Acciones { get; } = new();

    public bool FallaAlObtenerProveedores { get; set; }

    public (string? Busqueda, int? IdProveedor, string? Accion)?
        UltimaConsulta { get; private set; }

    public ProductoEscritura? UltimoProductoCreado { get; private set; }

    public int IdProductoCreado { get; set; } = 100;

    public (int IdProducto, ProductoEscritura Producto)?
        UltimaActualizacion { get; private set; }

    public int? UltimaBaja { get; private set; }

    public ResultadoBajaProducto ResultadoBajaADevolver { get; set; } =
        ResultadoBajaProducto.Descontinuado;

    public IReadOnlyList<ProductoInventario> Consultar(
        string? busqueda,
        int? idProveedor,
        string? accionTerapeutica)
    {
        UltimaConsulta = (
            busqueda,
            idProveedor,
            accionTerapeutica);

        return Array.Empty<ProductoInventario>();
    }

    public IReadOnlyList<int> ObtenerProveedores()
    {
        if (FallaAlObtenerProveedores)
        {
            throw new InvalidOperationException(
                "Falla simulada de la base de datos.");
        }

        return IdsDeProveedor;
    }

    public IReadOnlyList<string> ObtenerAccionesTerapeuticas() =>
        Acciones;

    public int Crear(ProductoEscritura producto)
    {
        UltimoProductoCreado = producto;
        return IdProductoCreado;
    }

    public void Actualizar(
        int idProducto,
        ProductoEscritura producto)
    {
        UltimaActualizacion = (
            idProducto,
            producto);
    }

    public ResultadoBajaProducto DarDeBaja(int idProducto)
    {
        UltimaBaja = idProducto;
        return ResultadoBajaADevolver;
    }
}