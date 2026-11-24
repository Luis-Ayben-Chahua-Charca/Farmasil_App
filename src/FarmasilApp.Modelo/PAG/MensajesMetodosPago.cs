namespace FarmasilApp.Modelo.PAG;

public static class MensajesMetodosPago
{
    public const string Titulo = "Gestión de Métodos de Pago";
    public const string MetodoRegistrado = "Método de pago registrado correctamente";
    public const string MetodoActualizado = "Método de pago actualizado correctamente";
    public const string ConfirmarBaja =
        "¿Está seguro de dar de baja este método de pago? Las ventas ya registradas con él no se verán afectadas.";
    public const string MetodoDadoDeBaja = "Método de pago dado de baja correctamente";

    public const string NombreVacio = "El nombre del método de pago es obligatorio.";
    public const string NombreDemasiadoLargo = "El nombre del método de pago no puede exceder los 50 caracteres.";
    public const string NombreRepetido = "Ya existe un método de pago con ese nombre.";
    public const string EstadoInvalido = "El estado debe ser Activo o Inactivo.";

    public const string ErrorConsulta = "No se pudieron consultar los métodos de pago. Inténtelo nuevamente.";
    public const string ErrorOperacion = "No se pudo completar la operación. Inténtelo nuevamente.";
}
