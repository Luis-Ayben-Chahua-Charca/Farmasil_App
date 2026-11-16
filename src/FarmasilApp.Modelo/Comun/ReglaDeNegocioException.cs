namespace FarmasilApp.Modelo.Comun;

/// <summary>
/// Una regla de negocio de la especificación cancela la operación (DEC-072). Es el único mecanismo con el que un
/// servicio del Modelo informa una validación o una guarda fallida, en todos los módulos.
/// </summary>
/// <remarks>
/// <para>
/// El mensaje es el texto exacto que muestra la especificación, tomado de la clase de constantes del módulo
/// (por ejemplo, <c>MensajesUsuarios</c>), para que las pruebas lo comparen sin copiarlo.
/// </para>
/// <para>
/// La VistaModelo captura esta excepción y muestra su mensaje tal cual. Cualquier otra excepción es un error técnico:
/// se registra con <c>Trace</c> y la pantalla muestra un mensaje genérico, nunca el texto técnico.
/// </para>
/// </remarks>
public sealed class ReglaDeNegocioException : Exception
{
    public ReglaDeNegocioException(string mensaje)
        : base(ValidarMensaje(mensaje))
    {
    }

    private static string ValidarMensaje(string mensaje)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mensaje);
        return mensaje;
    }
}
