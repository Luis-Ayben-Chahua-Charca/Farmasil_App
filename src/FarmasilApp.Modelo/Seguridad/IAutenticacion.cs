using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Inicio y cierre de la sesión activa (RNF-0006, condición 1; DEC-012). Lo usan solo la VistaModelo de inicio de
/// sesión y la ventana principal; los módulos de negocio ven únicamente <see cref="ISesion"/> (DEC-069).
/// </summary>
/// <remarks>
/// No pertenece a la matriz de interfaces entre módulos porque Usuarios es transversal. La implementa la misma clase
/// que <see cref="ISesion"/>, registrada como una sola instancia para las dos interfaces.
/// </remarks>
public interface IAutenticacion
{
    /// <summary>Si hay una sesión iniciada.</summary>
    bool HaySesion { get; }

    /// <summary>
    /// Verifica el usuario y la contraseña contra el hash con sal y abre la sesión con el rol de la cuenta.
    /// </summary>
    /// <exception cref="ReglaDeNegocioException">
    /// Credenciales incorrectas (<see cref="MensajesSesion.CredencialesIncorrectas"/>) o cuenta 'Inactivo' con la
    /// contraseña correcta (<see cref="MensajesSesion.CuentaInactiva"/>). La sesión no cambia.
    /// </exception>
    void IniciarSesion(string? nombreUsuario, string? contrasena);

    /// <summary>Borra la sesión. Sin sesión no hace nada.</summary>
    void CerrarSesion();
}
