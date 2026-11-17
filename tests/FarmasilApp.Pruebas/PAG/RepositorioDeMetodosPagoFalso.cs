using FarmasilApp.Modelo.PAG;

namespace FarmasilApp.Pruebas.PAG;

internal sealed class RepositorioDeMetodosPagoFalso : IMetodosPagoRepositorio
{
    public List<MetodoPagoConsulta> Resultados { get; } = new();

    public (string? NombreMetodo, string? Estado)? UltimaConsulta { get; private set; }

    public bool FallaAlConsultar { get; set; }

    public IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado)
    {
        if (FallaAlConsultar)
        {
            throw new InvalidOperationException("Falla simulada de la base de datos.");
        }

        UltimaConsulta = (nombreMetodo, estado);
        return Resultados;
    }
}
