using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.REA;

/// <summary>
/// Ítem de la lista de reabastecimiento que ofrece <see cref="IReabastecimiento"/> (TBL_LISTA_REABASTECIMIENTO).
/// </summary>
public sealed class ItemReabastecimientoDto
{
    public int IdProducto { get; init; }

    public OrigenReabastecimiento Origen { get; init; }

    public Prioridad Prioridad { get; init; }

    public DateOnly FechaInclusion { get; init; }
}
