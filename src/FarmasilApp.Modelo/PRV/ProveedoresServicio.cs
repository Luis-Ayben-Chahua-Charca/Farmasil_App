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
        var busquedaNormalizada = Normalizar(busqueda);
        var estadoNormalizado = Normalizar(estado);

        if (estadoNormalizado is not null
            && estadoNormalizado != "Activo"
            && estadoNormalizado != "Inactivo")
        {
            throw new ArgumentException(
                "El estado debe ser Activo o Inactivo.", nameof(estado));
        }

        return _repositorio.Consultar(busquedaNormalizada, estadoNormalizado);
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
