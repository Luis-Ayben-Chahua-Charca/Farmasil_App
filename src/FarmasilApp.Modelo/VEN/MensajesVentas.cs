namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Textos de la pantalla de venta (ESP-0001). Son constantes públicas para que la VistaModelo y las pruebas usen
/// exactamente el mismo texto (DEC-072).
/// </summary>
/// <remarks>
/// Los textos con referencia a la ESP son literales de la especificación y no se cambian sin cambiarla. Los demás no los
/// fija ninguna especificación y se listan en la descripción del PR del #89 para su aprobación.
/// </remarks>
public static class MensajesVentas
{
    public const string Titulo = "Venta";

    /// <summary>ESP-0001, remesa con <c>fecha_vencimiento</c> hasta la fecha de la venta.</summary>
    public const string RemesaVencida = "La remesa está vencida y no puede venderse";

    /// <summary>ESP-0001, remesa con <c>estado_lote</c> distinto de 'Disponible'.</summary>
    public const string RemesaConAlerta = "La remesa tiene una alerta activa y no puede venderse";

    /// <summary>La remesa dejó de existir entre que se mostró y se agregó. Texto no fijado por la ESP.</summary>
    public const string RemesaNoDisponible = "La remesa ya no está disponible para la venta.";

    /// <summary>ESP-0001: <c>VALIDAR cantidad = ENTERO &gt; 0</c>. Texto no fijado por la ESP.</summary>
    public const string CantidadInvalida = "Ingrese una cantidad entera mayor que cero.";

    /// <summary>ESP-0001: <c>VALIDAR cantidadTotal &lt;= lote.stock_actual</c>. Texto no fijado; {0} es el stock de la remesa.</summary>
    public const string StockInsuficiente = "La cantidad supera el stock de la remesa ({0} unidades).";

    /// <summary>Se intentó agregar sin elegir un medicamento. Texto no fijado por la ESP.</summary>
    public const string SinMedicamento = "Seleccione un medicamento del catálogo.";

    /// <summary>Se intentó agregar sin remesa. Texto no fijado por la ESP.</summary>
    public const string SinRemesa = "No hay una remesa vendible para este medicamento.";

    /// <summary>Sesión sin rol de venta (RNF-0006, condición 3). Texto no fijado por la ESP.</summary>
    public const string AccesoDenegado = "Su sesión no permite registrar ventas.";

    /// <summary>
    /// Motivo temporal del botón «Registrar venta», que queda deshabilitado (RNF-0006, condición 4; DEC-075). Se quita
    /// cuando el botón se conecte.
    /// </summary>
    public const string RegistroNoDisponible = "El registro de la venta todavía no está disponible";

    /// <summary>Error técnico al cargar el catálogo o las remesas. Texto no fijado por la ESP.</summary>
    public const string ErrorCarga = "No se pudo cargar la información de venta. Inténtelo nuevamente.";

    /// <summary>Error técnico al agregar una línea. Texto no fijado por la ESP.</summary>
    public const string ErrorOperacion = "No se pudo completar la operación. Inténtelo nuevamente.";
}
