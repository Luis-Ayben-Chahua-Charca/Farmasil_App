namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Servicio de aplicación para ESP-0038.
/// </summary>
public sealed class ProveedoresServicio
{
    private readonly IProveedoresRepositorio _repositorio;

    public ProveedoresServicio(IProveedoresRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public IReadOnlyList<ProveedorConsulta> Consultar(
        string? busqueda,
        string? estado)
    {
        return _repositorio.Consultar(
            Normalizar(busqueda),
            Normalizar(estado));
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
