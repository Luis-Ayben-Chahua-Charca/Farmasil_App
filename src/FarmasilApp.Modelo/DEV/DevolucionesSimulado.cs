namespace FarmasilApp.Modelo.DEV;

/// <summary>
/// Implementación simulada de <see cref="IDevoluciones"/> mientras DEV no esté listo (plan, sección 10).
/// Devuelve siempre la orden pendiente de database/002_datos_prueba.sql: la orden 1, creada 2 días antes de hoy,
/// con 2 unidades de la remesa PAR-002 (id 2). La orden 2 está 'Completada' y no cuenta. No tiene estado.
/// </summary>
public sealed class DevolucionesSimulado : IDevoluciones
{
    private readonly TimeProvider _reloj;

    public DevolucionesSimulado(TimeProvider reloj)
    {
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    public IReadOnlyList<OrdenPendienteDto> OrdenesPendientesDeRemesa(int idLote)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idLote);

        if (idLote != 2)
        {
            return Array.Empty<OrdenPendienteDto>();
        }

        var hoy = DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);
        return new[]
        {
            new OrdenPendienteDto { IdDevolucion = 1, FechaCreacion = hoy.AddDays(-2), CantidadDevolver = 2 },
        };
    }
}
