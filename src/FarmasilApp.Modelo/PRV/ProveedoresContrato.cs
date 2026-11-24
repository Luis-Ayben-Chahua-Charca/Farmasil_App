using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Implementación real de <see cref="IProveedores"/> respaldada por la base de datos.
/// Sustituye a <see cref="ProveedoresSimulado"/> cuando el módulo PRV está listo (plan, sección 10).
/// </summary>
public sealed class ProveedoresContrato : IProveedores
{
    private readonly IProveedoresRepositorio _repositorio;

    public ProveedoresContrato(IProveedoresRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public IReadOnlyList<ProveedorDto> ProveedoresActivos()
    {
        return _repositorio.ObtenerActivos();
    }

    public ProveedorDto? ObtenerProveedor(int idProveedor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProveedor);

        return _repositorio.ObtenerPorId(idProveedor);
    }
}
