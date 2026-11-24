namespace FarmasilApp.Modelo.DOC;

/// <summary>Mensajes compartidos de DOC (ESP-0010 y DEC-072/079).</summary>
public static class MensajesComprobantes
{
    public const string TipoInvalido = "El tipo de comprobante debe ser Boleta o Factura.";
    public const string NumeroDemasiadoLargo = "El número de comprobante no puede superar 20 caracteres.";
    public const string FechaInvalida = "Ingrese una fecha de emisión válida.";
    public const string AccesoDenegado = "Su sesión no permite consultar comprobantes tributarios.";
    public const string IndicacionInicial = "Seleccione los filtros y presione Consultar. También puede consultar sin filtros.";
    public const string SinResultados = "No se encontraron comprobantes con los filtros indicados.";
    public const string CantidadEncontrada = "Se encontraron {0} comprobante(s).";
    public const string ErrorConsulta = "No se pudo realizar la consulta. Verifique la conexión con DB_FARMASIL.";
    // Registro de comprobantes (ESP-0009).

public const string NumeroObligatorio =
    "Ingrese el número de comprobante.";

public const string NumeroDuplicado =
    "El número de comprobante ya se encuentra registrado.";

public const string VentaYaEmitida =
    "La venta seleccionada ya tiene un comprobante emitido.";

public const string VentaNoEncontrada =
    "La venta seleccionada no existe.";

public const string RegistroCorrecto =
    "Comprobante registrado correctamente";

public const string ErrorRegistro =
    "No se pudo registrar el comprobante. Verifique la conexión con DB_FARMASIL.";
}
