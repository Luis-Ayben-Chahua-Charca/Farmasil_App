namespace FarmasilApp.Modelo.DOC;

/// <summary>
/// Datos de la previsualización que DOC entrega al servicio PDF.
/// Contiene únicamente información existente en FarmasilApp; no representa
/// una factura electrónica autorizada ni datos enviados a SUNAT/OSE.
/// </summary>
public sealed class DatosComprobantePdf
{
    public string TipoComprobante { get; init; } = string.Empty;

    public string NumeroComprobante { get; init; } = string.Empty;

    public DateTime FechaEmision { get; init; }

    public string CodigoVenta { get; init; } = string.Empty;

    public DateTime FechaVenta { get; init; }

    public decimal MontoTotal { get; init; }

    public IReadOnlyList<DetalleComprobantePdf> Detalle { get; init; }
        = Array.Empty<DetalleComprobantePdf>();
}

/// <summary>Una línea del detalle usado en el borrador PDF.</summary>
public sealed class DetalleComprobantePdf
{
    public string NombreProducto { get; init; } = string.Empty;

    public string NumeroLote { get; init; } = string.Empty;

    public int Cantidad { get; init; }

    public decimal PrecioUnitario { get; init; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}
