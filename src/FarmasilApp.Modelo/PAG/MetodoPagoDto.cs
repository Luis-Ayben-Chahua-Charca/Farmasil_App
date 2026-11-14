namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Método de pago activo que ofrece <see cref="IPagos"/> (TBL_METODOS_PAGO).
/// </summary>
public sealed class MetodoPagoDto
{
    public int IdMetodoPago { get; init; }

    public string Nombre { get; init; } = string.Empty;
}
