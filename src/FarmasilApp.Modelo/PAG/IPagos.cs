namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Interfaz provista por Métodos de Pago. La usa Ventas (ESP-0001) para el combo de método de pago.
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface IPagos
{
    /// <summary>Métodos de pago con estado 'Activo', en el orden de su identificador.</summary>
    IReadOnlyList<MetodoPagoDto> MetodosActivos();
}
