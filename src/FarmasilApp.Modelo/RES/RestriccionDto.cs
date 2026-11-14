using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.RES;

/// <summary>
/// Restricción de venta activa que ofrece <see cref="IRestricciones"/> (TBL_RESTRICCIONES_VENTA).
/// </summary>
public sealed class RestriccionDto
{
    public int IdRestriccion { get; init; }

    public int IdProducto { get; init; }

    public string CondicionCliente { get; init; } = string.Empty;

    public TipoRestriccion Tipo { get; init; }

    public string MotivoAdvertencia { get; init; } = string.Empty;
}
