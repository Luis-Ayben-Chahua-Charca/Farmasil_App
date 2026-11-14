namespace FarmasilApp.Modelo.REA;

/// <summary>
/// Interfaz provista por Reabastecimiento. La usa Comparación de Precios para incluir solo los medicamentos pendientes (ESP-0050).
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface IReabastecimiento
{
    /// <summary>Medicamentos de la lista de reabastecimiento, con su origen y su prioridad, por identificador de medicamento.</summary>
    IReadOnlyList<ItemReabastecimientoDto> ListaReabastecimiento();
}
