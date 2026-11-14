namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Implementación simulada de <see cref="IPagos"/> mientras PAG no esté listo (plan, sección 10).
/// Devuelve siempre los métodos activos de database/002_datos_prueba.sql, con los mismos ids
/// (los que tiene una base recién creada). No tiene estado.
/// </summary>
public sealed class PagosSimulado : IPagos
{
    private static readonly IReadOnlyList<MetodoPagoDto> Activos = Array.AsReadOnly(new[]
    {
        new MetodoPagoDto { IdMetodoPago = 1, Nombre = "Efectivo" },
        new MetodoPagoDto { IdMetodoPago = 2, Nombre = "Yape" },
    });

    public IReadOnlyList<MetodoPagoDto> MetodosActivos()
    {
        return Activos;
    }
}
