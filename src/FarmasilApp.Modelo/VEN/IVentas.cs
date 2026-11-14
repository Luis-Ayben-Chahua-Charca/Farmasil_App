namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Interfaz provista por Ventas. La usan Documentación Tributaria (ESP-0009) y Reportes de Ventas (ESP-0021).
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069). Es de solo lectura: no modifica ninguna venta.
/// </summary>
public interface IVentas
{
    /// <summary>
    /// Ventas que cumplen el filtro, por identificador ascendente, cada una con su detalle.
    /// Sin criterios se devuelven todas.
    /// </summary>
    /// <exception cref="ArgumentNullException">El filtro es nulo; para no filtrar se pasa un <see cref="FiltroVentas"/> vacío.</exception>
    /// <exception cref="ArgumentException"><c>Desde</c> es posterior a <c>Hasta</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><c>IdProducto</c> no es positivo.</exception>
    IReadOnlyList<VentaDto> ConsultarVentas(FiltroVentas filtro);

    /// <summary>Venta con ese identificador o <c>null</c> si no existe.</summary>
    /// <exception cref="ArgumentOutOfRangeException">El identificador no es positivo.</exception>
    VentaDto? ObtenerVenta(int idVenta);
}
