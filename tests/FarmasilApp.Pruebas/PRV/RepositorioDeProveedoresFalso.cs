using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

/// <summary>Repositorio de Proveedores de prueba: devuelve lo que se le configura y anota los criterios de la última consulta.</summary>
internal sealed class RepositorioDeProveedoresFalso : IProveedoresRepositorio
{
    public List<ProveedorConsulta> Resultados { get; } = new();

    public (string? Busqueda, string? Estado)? UltimaConsulta { get; private set; }

    public bool FallaAlConsultar { get; set; }

    public IReadOnlyList<ProveedorConsulta> Consultar(string? busqueda, string? estado)
    {
        if (FallaAlConsultar)
        {
            throw new InvalidOperationException("Falla simulada de la base de datos.");
        }

        UltimaConsulta = (busqueda, estado);
        return Resultados;
    }
}
