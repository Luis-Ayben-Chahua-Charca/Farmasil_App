namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Interfaz provista por Lotes. La usan Ventas, Alertas de Vencimiento, Devoluciones y Reabastecimiento.
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069). Una remesa es vendible cuando el medicamento y
/// la remesa están 'Disponible', <c>stock_actual</c> es mayor que cero y el vencimiento es posterior a la fecha (DEC-033).
/// </summary>
/// <remarks>
/// Las operaciones que escriben (<see cref="Descontar"/>, <see cref="Restituir"/>, <see cref="Bloquear"/> y
/// <see cref="Liberar"/>) participan de la transacción que tenga abierta quien las llama: no abren una propia (DEC-069).
/// Las implementaciones reales lanzan <see cref="InvalidOperationException"/> si la remesa no existe o el stock no alcanza.
/// Todas las operaciones rechazan identificadores y cantidades no positivos con <see cref="ArgumentOutOfRangeException"/>.
/// </remarks>
public interface ILotes
{
    /// <summary>Medicamentos con alguna remesa vendible, por identificador, con su stock vendible (DEC-044).</summary>
    IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles();

    /// <summary>
    /// Remesas vendibles del medicamento por vencimiento ascendente (criterio FEFO). La primera es la sugerida.
    /// La lista está vacía si el medicamento no tiene remesas vendibles.
    /// </summary>
    IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto);

    /// <summary>
    /// Datos de las remesas pedidas, en el orden en que se piden (DEC-057). Los identificadores que no existen no se devuelven.
    /// </summary>
    /// <exception cref="ArgumentNullException">La colección es nula.</exception>
    IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote);

    /// <summary>
    /// Remesas del proveedor en estado 'Disponible' o 'Bloqueado por devolucion', con stock mayor que cero,
    /// por vencimiento ascendente. Alimenta <c>DEV-CMB-LOTE-DEVOLUCION</c> (ESP-0025).
    /// </summary>
    IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor);

    /// <summary>
    /// Remesas 'Disponible' con stock mayor que cero y vencimiento hasta la fecha límite (inclusive, sin cota inferior),
    /// por vencimiento ascendente. Alertas calcula la fecha límite con su propio umbral (ESP-0013, RNF-0010).
    /// </summary>
    IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite);

    /// <summary>
    /// Remesas en estado 'Disponible' o 'Bloqueado por devolucion', con stock mayor que cero, de cualquier proveedor,
    /// por vencimiento ascendente. Alimenta <c>ALV-CMB-LOTE-ALERTA</c> (ESP-0014): una remesa ya bloqueada admite otra alerta.
    /// </summary>
    IReadOnlyList<DatosRemesaDto> RemesasDisponibles();

    /// <summary>Resta existencias y aplica la regla única del estado de la remesa (DEC-034).</summary>
    void Descontar(int idLote, int cantidad);

    /// <summary>Suma existencias y aplica la regla única del estado de la remesa (DEC-034).</summary>
    void Restituir(int idLote, int cantidad);

    /// <summary>Deja la remesa en 'Bloqueado por devolucion' (ESP-0014).</summary>
    void Bloquear(int idLote);

    /// <summary>
    /// Deja la remesa 'Disponible' si tiene stock y 'Agotado' si no (ESP-0016, DEC-034).
    /// Quien llama verifica antes que la remesa no esté vencida ni tenga otra alerta vigente.
    /// </summary>
    void Liberar(int idLote);
}
