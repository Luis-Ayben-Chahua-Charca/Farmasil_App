namespace FarmasilApp.Modelo.DOC;

/// <summary>Mensajes compartidos de DOC (ESP-0009 a ESP-0012 y DEC-072/079).</summary>
public static class MensajesComprobantes
{
    public const string TipoInvalido = "El tipo de comprobante debe ser Boleta o Factura.";
    public const string NumeroObligatorio = "Ingrese el número de comprobante.";
    public const string NumeroDemasiadoLargo = "El número de comprobante no puede superar 20 caracteres.";
    public const string NumeroDuplicado = "El número de comprobante ya se encuentra registrado.";
    public const string FechaInvalida = "Ingrese una fecha de emisión válida.";
    public const string AccesoDenegado = "Su sesión no permite consultar comprobantes tributarios.";
    public const string AccesoAdministracionDenegado = "Solo el Administrador puede actualizar o anular comprobantes tributarios.";
    public const string IndicacionInicial = "Seleccione los filtros y presione Consultar. También puede consultar sin filtros.";
    public const string SinResultados = "No se encontraron comprobantes con los filtros indicados.";
    public const string CantidadEncontrada = "Se encontraron {0} comprobante(s).";
    public const string ErrorConsulta = "No se pudo realizar la consulta. Verifique la conexión con DB_FARMASIL.";

    // Registro (ESP-0009).
    public const string VentaYaEmitida = "La venta seleccionada ya tiene un comprobante emitido.";
    public const string VentaNoEncontrada = "La venta seleccionada no existe.";
    public const string RegistroCorrecto = "Comprobante registrado correctamente";
    public const string ErrorRegistro = "No se pudo registrar el comprobante. Verifique la conexión con DB_FARMASIL.";

    // Actualización y anulación (ESP-0011/ESP-0012).
    public const string ComprobanteNoEncontrado = "No se encontró el comprobante seleccionado.";
    public const string ComprobanteAnuladoNoEditable = "El comprobante está anulado y no admite modificación.";
    public const string ComprobanteNoEmitido = "Solo se puede actualizar o anular un comprobante con estado Emitido.";
    public const string ActualizacionCorrecta = "Comprobante actualizado correctamente.";
    public const string AnulacionCorrecta = "Comprobante anulado correctamente.";
    public const string ConfirmarAnulacion = "¿Está seguro de anular este comprobante? El documento se conservará con estado Anulado.";
    public const string ErrorActualizacion = "No se pudo actualizar el comprobante. Verifique la conexión con DB_FARMASIL.";
    public const string ErrorAnulacion = "No se pudo anular el comprobante. Verifique la conexión con DB_FARMASIL.";

    // PDF de previsualización.
    public const string ErrorPdf = "No se pudo generar el borrador PDF. Verifique los permisos de la carpeta Documentos.";
}
