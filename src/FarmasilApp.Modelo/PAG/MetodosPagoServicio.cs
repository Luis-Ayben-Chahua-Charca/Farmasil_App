namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Servicio de aplicación para ESP-0018.
/// </summary>
public sealed class MetodosPagoServicio
{
    private readonly IMetodosPagoRepositorio _repositorio;

    public MetodosPagoServicio(IMetodosPagoRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado)
    {
        var nombreNormalizado = Normalizar(nombreMetodo);
        var estadoNormalizado = Normalizar(estado);

        if (estadoNormalizado is not null
            && estadoNormalizado != "Activo"
            && estadoNormalizado != "Inactivo")
        {
            throw new ArgumentException(
                "El estado debe ser Activo o Inactivo.",
                nameof(estado));
        }

        return _repositorio.Consultar(nombreNormalizado, estadoNormalizado);
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
