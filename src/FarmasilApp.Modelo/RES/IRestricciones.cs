namespace FarmasilApp.Modelo.RES;

/// <summary>
/// Interfaz provista por Restricciones de Venta. La usa Ventas al agregar una línea (ESP-0001).
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface IRestricciones
{
    /// <summary>
    /// Todas las restricciones con estado 'Activo' del medicamento, con su tipo y su motivo.
    /// Ventas decide: si alguna es bloqueante rechaza la línea, y las informativas solo se muestran (DEC-033).
    /// La lista está vacía si el medicamento no tiene restricciones activas.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">El identificador no es positivo.</exception>
    IReadOnlyList<RestriccionDto> RestriccionesActivas(int idProducto);
}
