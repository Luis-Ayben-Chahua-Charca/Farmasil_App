namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Mensajes funcionales del módulo LOT.
/// Los textos definidos explícitamente por las ESP se conservan literales.
/// </summary>
public static class MensajesLotes
{
    public const string RemesaObligatoria =
        "Seleccione una remesa.";

    public const string ProductoObligatorio =
        "Seleccione un medicamento.";

    public const string ProductoNoDisponible =
        "El medicamento seleccionado no está disponible.";

    public const string ProveedorObligatorio =
        "Seleccione un proveedor.";

    public const string ProveedorNoExiste =
        "El proveedor seleccionado no existe.";

    public const string NumeroLoteObligatorio =
        "El número de lote es obligatorio.";

    public const string NumeroLoteDuplicado =
        "Ya existe una remesa con ese número de lote para el medicamento seleccionado.";

    public const string FechaVencimientoInvalida =
        "La fecha de vencimiento debe ser posterior a la fecha actual.";

    public const string StockInicialInvalido =
        "Las existencias iniciales deben ser un entero mayor que cero.";

    public const string StockInvalido =
        "Las existencias deben ser un entero mayor o igual a cero.";

    public const string FechaIngresoInvalida =
        "La fecha de ingreso no puede ser posterior a la fecha actual.";

    public const string ProveedorConDevolucion =
        "El proveedor no puede cambiarse: la remesa figura en una orden de devolución";

    public const string StockMenorQueComprometido =
        "Las existencias no pueden ser inferiores a las unidades comprometidas en devoluciones pendientes";

    public const string DevolucionPendiente =
        "La remesa está comprometida en una orden de devolución pendiente";

    public const string ConfirmarBaja =
        "¿Está seguro de dar de baja esta remesa? El histórico de ventas que la involucra se conservará.";

    public const string Registrada =
        "Remesa registrada correctamente";

    public const string Actualizada =
        "Remesa actualizada correctamente";

    public const string DadaDeBaja =
        "Remesa dada de baja correctamente";
}
