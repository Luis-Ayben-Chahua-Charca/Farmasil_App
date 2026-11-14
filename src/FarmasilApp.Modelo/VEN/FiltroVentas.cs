namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Criterios de <see cref="IVentas.ConsultarVentas"/>. Todos son opcionales y se combinan con «y».
/// </summary>
public sealed class FiltroVentas
{
    /// <summary>Primer día incluido, desde las 00:00.</summary>
    public DateOnly? Desde { get; init; }

    /// <summary>Último día incluido, hasta el final del día.</summary>
    public DateOnly? Hasta { get; init; }

    /// <summary>Solo las ventas con alguna remesa de este medicamento.</summary>
    public int? IdProducto { get; init; }
}
