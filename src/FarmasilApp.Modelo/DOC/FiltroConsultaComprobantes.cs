namespace FarmasilApp.Modelo.DOC;

/// <summary>
/// Filtros opcionales definidos por ESP-0010 v03.00.
/// Un valor nulo significa que el filtro no se aplica.
/// </summary>
public sealed class FiltroConsultaComprobantes
{
    public string? TipoComprobante { get; init; }

    public string? NumeroComprobante { get; init; }

    public DateTime? FechaEmision { get; init; }
}
