namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Método de pago mostrado por la consulta (ESP-0018).
/// </summary>
public sealed class MetodoPagoConsulta
{
    public int IdMetodoPago { get; init; }

    public string NombreMetodo { get; init; } = string.Empty;

    public string Estado { get; init; } = string.Empty;
}
