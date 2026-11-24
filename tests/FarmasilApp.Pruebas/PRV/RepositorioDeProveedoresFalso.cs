using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

/// <summary>
/// Repositorio de Proveedores de prueba: devuelve lo que se le configura y anota los datos de cada operación.
/// </summary>
internal sealed class RepositorioDeProveedoresFalso : IProveedoresRepositorio
{
    // --- Consultar ---
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

    // --- ObtenerActivos ---
    public List<ProveedorDto> ProveedoresActivos { get; } = new();

    public IReadOnlyList<ProveedorDto> ObtenerActivos() => ProveedoresActivos;

    // --- ObtenerPorId ---
    public Dictionary<int, ProveedorDto> ProveedoresPorId { get; } = new();

    public ProveedorDto? ObtenerPorId(int idProveedor)
    {
        return ProveedoresPorId.TryGetValue(idProveedor, out var proveedor) ? proveedor : null;
    }

    // --- ExisteRuc ---
    public string? RucExistente { get; set; }

    public bool ExisteRuc(string ruc, int? excluirIdProveedor = null)
    {
        return ruc == RucExistente;
    }

    // --- TieneOrdenesPendientes ---
    public bool OrdenesDevPendientes { get; set; }

    public bool TieneOrdenesPendientes(int idProveedor) => OrdenesDevPendientes;

    // --- Crear ---
    public ProveedorEscritura? UltimoCreado { get; private set; }
    public int IdProveedorADevolver { get; set; } = 100;
    public bool FallaAlCrear { get; set; }

    public int Crear(ProveedorEscritura proveedor)
    {
        if (FallaAlCrear)
        {
            throw new ReglaDeNegocioException(MensajesProveedores.RucRepetido);
        }

        UltimoCreado = proveedor;
        return IdProveedorADevolver;
    }

    // --- Actualizar ---
    public (int IdProveedor, ProveedorEscritura Proveedor)? UltimaActualizacion { get; private set; }

    public void Actualizar(int idProveedor, ProveedorEscritura proveedor)
    {
        UltimaActualizacion = (idProveedor, proveedor);
    }

    // --- DarDeBaja ---
    public int? UltimaBaja { get; private set; }
    public ResultadoBajaProveedor ResultadoBajaADevolver { get; set; } = ResultadoBajaProveedor.Eliminado;

    public ResultadoBajaProveedor DarDeBaja(int idProveedor)
    {
        UltimaBaja = idProveedor;
        return ResultadoBajaADevolver;
    }
}
