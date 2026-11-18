namespace FarmasilApp.Modelo.ALV;

public sealed class ResultadoAlertas
{
    public int UmbralMeses { get; init; }
    public DateOnly FechaLimite { get; init; }
    /// <summary>Conteo completo del proceso 1 de RNF-0010, sin filtros de interfaz.</summary>
    public int TotalProximasAVencer { get; init; }
    public IReadOnlyList<AlertaConsulta> Alertas { get; init; } = Array.Empty<AlertaConsulta>();
}
