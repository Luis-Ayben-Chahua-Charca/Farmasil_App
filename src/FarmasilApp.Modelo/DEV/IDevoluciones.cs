namespace FarmasilApp.Modelo.DEV;

/// <summary>
/// Interfaz provista por Devoluciones. La usa Alertas de Vencimiento (ESP-0016) para no eliminar una alerta
/// cuya remesa está en una orden pendiente. Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface IDevoluciones
{
    /// <summary>
    /// Órdenes 'Pendiente' que incluyen la remesa, por identificador ascendente.
    /// La lista está vacía si la remesa no está en ninguna orden pendiente, o si no existe.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">El identificador no es positivo.</exception>
    IReadOnlyList<OrdenPendienteDto> OrdenesPendientesDeRemesa(int idLote);
}
