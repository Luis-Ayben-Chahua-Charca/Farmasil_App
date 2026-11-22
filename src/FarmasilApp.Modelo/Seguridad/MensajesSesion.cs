namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Textos del inicio y cierre de sesión (RNF-0006, condición 1; DEC-012). Son constantes públicas para que la
/// VistaModelo y las pruebas usen exactamente el mismo texto (DEC-072).
/// </summary>
/// <remarks>
/// Ninguna especificación fija estos textos: los aprobó la instancia maestra el 22/11/2026 (DEC-080) y se listan en la
/// descripción del PR del #88. Por la condición 5 de RNF-0006, ninguno incluye ni se acompaña de la contraseña.
/// </remarks>
public static class MensajesSesion
{
    /// <summary>Título de la pantalla de inicio de sesión.</summary>
    public const string Titulo = "Inicio de sesión";

    /// <summary>Usuario inexistente o contraseña errónea: el mismo texto para ambos, para no revelar qué usuarios existen.</summary>
    public const string CredencialesIncorrectas = "Usuario o contraseña incorrectos.";

    /// <summary>Cuenta 'Inactivo' con la contraseña correcta (RNF-0006, condición 1). Solo lo ve quien ya conoce la contraseña.</summary>
    public const string CuentaInactiva = "La cuenta está inactiva. Consulte con un administrador.";

    /// <summary>Se pidió el usuario de la sesión sin haber iniciado sesión (RNF-0006, condición 3).</summary>
    public const string SinSesion = "No hay una sesión iniciada.";

    /// <summary>Error técnico al iniciar sesión (por ejemplo, base de datos no disponible). Texto no fijado por la maestra.</summary>
    public const string ErrorInicio = "No se pudo iniciar sesión. Inténtelo nuevamente.";
}
