namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Operaciones de persistencia necesarias para la consulta de métodos de pago.
/// </summary>
public interface IMetodosPagoRepositorio
{
    IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado);
}
