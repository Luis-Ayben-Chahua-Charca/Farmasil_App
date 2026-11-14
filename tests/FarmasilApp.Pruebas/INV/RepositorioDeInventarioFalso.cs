using FarmasilApp.Modelo.INV;

namespace FarmasilApp.Pruebas.INV;

/// <summary>Repositorio de Inventario de prueba: devuelve lo que se le configura y anota los criterios de la última consulta.</summary>
internal sealed class RepositorioDeInventarioFalso : IInventarioRepositorio
{
    public List<int> IdsDeProveedor { get; } = new();

    public List<string> Acciones { get; } = new();

    public bool FallaAlObtenerProveedores { get; set; }

    public (string? Busqueda, int? IdProveedor, string? Accion)? UltimaConsulta { get; private set; }

    public IReadOnlyList<ProductoInventario> Consultar(string? busqueda, int? idProveedor, string? accionTerapeutica)
    {
        UltimaConsulta = (busqueda, idProveedor, accionTerapeutica);
        return Array.Empty<ProductoInventario>();
    }

    public IReadOnlyList<int> ObtenerProveedores()
    {
        if (FallaAlObtenerProveedores)
        {
            throw new InvalidOperationException("Falla simulada de la base de datos.");
        }

        return IdsDeProveedor;
    }

    public IReadOnlyList<string> ObtenerAccionesTerapeuticas() => Acciones;
}
