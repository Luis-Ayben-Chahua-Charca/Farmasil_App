namespace FarmasilApp.Modelo.DOC;

/// <summary>
/// Comprobante obtenido mediante la consulta ESP-0010.
/// El importe total pertenece a TBL_REGISTRO_VENTAS, no a la tabla de comprobantes.
/// </summary>
public sealed class ComprobanteTributario
{
    public int IdComprobante { get; init; }

    public string NumeroComprobante { get; init; } = string.Empty;

    public string TipoComprobante { get; init; } = string.Empty;

    public DateTime FechaEmision { get; init; }

    public int IdVenta { get; init; }

    public decimal MontoTotal { get; init; }

    public string EstadoDocumento { get; init; } = string.Empty;
}
