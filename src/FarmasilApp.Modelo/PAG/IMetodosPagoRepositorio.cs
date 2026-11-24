namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Operaciones de persistencia para consultar y mantener métodos de pago.
/// </summary>
public interface IMetodosPagoRepositorio
{
    IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado);

    bool ExisteNombre(string nombreMetodo, int? excluirIdMetodoPago);

    int Crear(string nombreMetodo, string estado);

    void Actualizar(int idMetodoPago, string nombreMetodo, string estado);

    ResultadoBajaMetodoPago DarDeBaja(int idMetodoPago);
}
