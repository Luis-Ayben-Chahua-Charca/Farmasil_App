using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Implementación simulada de <see cref="IVentas"/> mientras VEN no esté listo (plan, sección 10).
/// Devuelve siempre las tres ventas históricas de database/002_datos_prueba.sql, con los mismos ids, hechas 10, 40 y 5 días
/// antes de hoy. No tiene estado: no registra ventas.
/// </summary>
public sealed class VentasSimulado : IVentas
{
    private readonly TimeProvider _reloj;

    public VentasSimulado(TimeProvider reloj)
    {
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    public IReadOnlyList<VentaDto> ConsultarVentas(FiltroVentas filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);
        if (filtro.Desde > filtro.Hasta)
        {
            throw new ArgumentException("La fecha inicial no puede ser posterior a la final.", nameof(filtro));
        }

        if (filtro.IdProducto is { } idProducto)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProducto);
        }

        var ahora = Ahora();
        var lotes = DatosDePrueba.Lotes(DateOnly.FromDateTime(ahora));

        return Ventas(ahora)
            .Where(venta => filtro.Desde is null || DateOnly.FromDateTime(venta.FechaHora) >= filtro.Desde)
            .Where(venta => filtro.Hasta is null || DateOnly.FromDateTime(venta.FechaHora) <= filtro.Hasta)
            .Where(venta => filtro.IdProducto is null
                || venta.Detalle.Any(linea => lotes.Single(lote => lote.IdLote == linea.IdLote).IdProducto == filtro.IdProducto))
            .OrderBy(venta => venta.IdVenta)
            .ToArray();
    }

    public VentaDto? ObtenerVenta(int idVenta)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idVenta);

        return Ventas(Ahora()).FirstOrDefault(venta => venta.IdVenta == idVenta);
    }

    /// <summary>Ahora, sin fracciones de segundo, como DATETIME2(0) en el script.</summary>
    private DateTime Ahora()
    {
        var ahora = _reloj.GetLocalNow().DateTime;
        return ahora.AddTicks(-(ahora.Ticks % TimeSpan.TicksPerSecond));
    }

    private static VentaDto[] Ventas(DateTime ahora)
    {
        return new[]
        {
            new VentaDto
            {
                IdVenta = 1,
                FechaHora = ahora.AddDays(-10),
                MontoTotal = 5.00m,
                IdMetodoPago = 1,
                IdUsuario = 2,
                Detalle = new[] { new DetalleVentaDto { IdLote = 1, Cantidad = 2, PrecioUnitario = 2.50m } },
            },
            new VentaDto
            {
                IdVenta = 2,
                FechaHora = ahora.AddDays(-40),
                MontoTotal = 4.00m,
                IdMetodoPago = 1,
                IdUsuario = 2,
                Detalle = new[] { new DetalleVentaDto { IdLote = 3, Cantidad = 1, PrecioUnitario = 4.00m } },
            },
            new VentaDto
            {
                IdVenta = 3,
                FechaHora = ahora.AddDays(-5),
                MontoTotal = 2.50m,
                IdMetodoPago = 2,
                IdUsuario = 1,
                Detalle = new[] { new DetalleVentaDto { IdLote = 1, Cantidad = 1, PrecioUnitario = 2.50m } },
            },
        };
    }
}
