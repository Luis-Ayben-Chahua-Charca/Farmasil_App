using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.REA;

/// <summary>
/// Implementación simulada de <see cref="IReabastecimiento"/> mientras REA no esté listo (plan, sección 10).
/// Devuelve siempre los tres ítems de database/002_datos_prueba.sql, incluidos hoy: ibuprofeno (2, automático y urgente),
/// loratadina (4, manual y normal) y omeprazol (6, automático y urgente). No tiene estado: no genera la lista.
/// </summary>
public sealed class ReabastecimientoSimulado : IReabastecimiento
{
    private readonly TimeProvider _reloj;

    public ReabastecimientoSimulado(TimeProvider reloj)
    {
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    public IReadOnlyList<ItemReabastecimientoDto> ListaReabastecimiento()
    {
        var hoy = DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);
        return new[]
        {
            new ItemReabastecimientoDto { IdProducto = 2, Origen = OrigenReabastecimiento.Automatico, Prioridad = Prioridad.Urgente, FechaInclusion = hoy },
            new ItemReabastecimientoDto { IdProducto = 4, Origen = OrigenReabastecimiento.Manual, Prioridad = Prioridad.Normal, FechaInclusion = hoy },
            new ItemReabastecimientoDto { IdProducto = 6, Origen = OrigenReabastecimiento.Automatico, Prioridad = Prioridad.Urgente, FechaInclusion = hoy },
        };
    }
}
