using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.USR;

/// <summary>
/// Cuenta de usuario tal como la devuelve la consulta (ESP-0034) y como la carga la ficha.
/// </summary>
/// <remarks>
/// No tiene ninguna propiedad de contraseña: el tipo garantiza que ni la contraseña ni su hash salgan del módulo
/// hacia la interfaz (ESP-0034; RNF-0006, condición 5). <see cref="IdUsuario"/> es la clave de selección y no se
/// muestra; la clave visible es <see cref="NombreUsuario"/>, que es único (DEC-068).
/// </remarks>
public sealed class CuentaUsuario
{
    public int IdUsuario { get; init; }

    public string NombreUsuario { get; init; } = string.Empty;

    public Rol Rol { get; init; }

    public EstadoRegistro Estado { get; init; }
}
