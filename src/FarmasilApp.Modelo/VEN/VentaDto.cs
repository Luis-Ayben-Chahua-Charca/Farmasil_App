namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Venta registrada con su detalle, tal como la ofrece <see cref="IVentas"/> a Documentación Tributaria y a Reportes
/// (TBL_REGISTRO_VENTAS y TBL_DETALLE_VENTAS).
/// </summary>
public sealed class VentaDto
{
    public int IdVenta { get; init; }

    public DateTime FechaHora { get; init; }

    public decimal MontoTotal { get; init; }

    public int IdMetodoPago { get; init; }

    /// <summary>Usuario que registró la venta. Su nombre se obtiene por la consulta transversal de Usuarios (DEC-059).</summary>
    public int IdUsuario { get; init; }

    public IReadOnlyList<DetalleVentaDto> Detalle { get; init; } = Array.Empty<DetalleVentaDto>();
}
