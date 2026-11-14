using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

/// <summary>Repositorio de Proveedores de prueba: devuelve lo que se le configura y anota los criterios de la última consulta.</summary>
internal sealed class RepositorioDeProveedoresFalso : IProveedoresRepositorio
{
    public List<ProveedorConsulta> Resultados { get; } = new();

    public (string? Busqueda, string? Estado)? UltimaConsulta { get; private set; }

    public IReadOnlyList<ProveedorConsulta> Consultar(string? busqueda, string? estado)
    {
        UltimaConsulta = (busqueda, estado);
        return Resultados;
    }
}
