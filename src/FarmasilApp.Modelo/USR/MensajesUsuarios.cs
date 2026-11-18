namespace FarmasilApp.Modelo.USR;

/// <summary>
/// Textos que muestra el módulo de Usuarios. Son constantes públicas para que la VistaModelo y las pruebas usen
/// exactamente el mismo texto (DEC-072).
/// </summary>
/// <remarks>
/// Los textos con referencia a una ESP son literales de la especificación y no se cambian sin cambiarla.
/// Los demás no los fija ninguna especificación y se listan en la descripción del PR del #68 para su aprobación.
/// </remarks>
public static class MensajesUsuarios
{
    /// <summary>Título de los mensajes de la pantalla (título del mockup ART-MKP-USR-0001).</summary>
    public const string Titulo = "Gestión de Usuarios";

    /// <summary>ESP-0033.</summary>
    public const string UsuarioRegistrado = "Usuario registrado correctamente";

    /// <summary>ESP-0035.</summary>
    public const string UsuarioActualizado = "Usuario actualizado correctamente";

    /// <summary>ESP-0035; RNF-0006, condición 6.</summary>
    public const string UltimoAdministrador = "El sistema debe conservar al menos una cuenta administradora activa";

    /// <summary>ESP-0035 v04.00 (DEC-073).</summary>
    public const string CuentaPropiaRolEstado =
        "No es posible cambiar el rol ni el estado de la cuenta con la que se encuentra abierta la sesión";

    /// <summary>ESP-0036.</summary>
    public const string CuentaPropiaBaja = "No es posible dar de baja la cuenta con la que se encuentra abierta la sesión";

    /// <summary>ESP-0036: texto de USR-TXT-MSJ en el modal USR-MDL-CONFIRMAR-ELIMINACION.</summary>
    public const string ConfirmarBaja =
        "¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará.";

    /// <summary>ESP-0036.</summary>
    public const string CuentaDadaDeBaja = "Cuenta dada de baja correctamente";

    /// <summary>ESP-0033 y ESP-0035: <c>VALIDAR nombreUsuario &lt;&gt; VACIO</c>. Texto no fijado por la ESP.</summary>
    public const string NombreVacio = "Ingrese el nombre de usuario.";

    /// <summary>ESP-0033 y ESP-0035: <c>VALIDAR LONGITUD(nombreUsuario) &lt;= 50</c> (DEC-074). Texto no fijado por la ESP.</summary>
    public const string NombreDemasiadoLargo = "El nombre de usuario no puede superar los 50 caracteres.";

    /// <summary>ESP-0033 y ESP-0035: <c>VALIDAR NO EXISTE(...)</c>. Texto no fijado por la ESP.</summary>
    public const string NombreRepetido = "Ya existe una cuenta con ese nombre de usuario.";

    /// <summary>ESP-0033: <c>VALIDAR contrasena &lt;&gt; VACIA</c>. Texto no fijado por la ESP.</summary>
    public const string ContrasenaVacia = "Ingrese la contraseña.";

    /// <summary>ESP-0033: <c>VALIDAR rol EN (...)</c>. Texto no fijado por la ESP.</summary>
    public const string RolInvalido = "Seleccione un rol válido.";

    /// <summary>ESP-0033: <c>VALIDAR estado EN (...)</c>. Texto no fijado por la ESP.</summary>
    public const string EstadoInvalido = "Seleccione un estado válido.";

    /// <summary>Sesión sin el rol 'Administrador' (RNF-0006, condición 3). Texto no fijado por la ESP.</summary>
    public const string AccesoDenegado = "Su rol no permite gestionar usuarios.";

    /// <summary>Error técnico al consultar. Texto no fijado por la ESP.</summary>
    public const string ErrorConsulta = "No se pudo consultar las cuentas. Inténtelo nuevamente.";

    /// <summary>Error técnico al registrar, actualizar o dar de baja. Texto no fijado por la ESP.</summary>
    public const string ErrorOperacion = "No se pudo completar la operación. Inténtelo nuevamente.";
}
