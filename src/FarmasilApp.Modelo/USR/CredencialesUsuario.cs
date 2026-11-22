using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.USR;

/// <summary>
/// Datos que necesita el inicio de sesión para verificar una cuenta: incluye el hash de la contraseña.
/// </summary>
/// <remarks>
/// Solo lo usa el Modelo (<c>SesionActual</c>). Nunca llega a la interfaz ni a otros módulos: por eso es distinto de
/// <see cref="CuentaUsuario"/>, que no tiene el hash (ESP-0034; RNF-0006, condición 5).
/// </remarks>
public sealed class CredencialesUsuario
{
    public int IdUsuario { get; init; }

    public string NombreUsuario { get; init; } = string.Empty;

    public string Hash { get; init; } = string.Empty;

    public Rol Rol { get; init; }

    public EstadoRegistro Estado { get; init; }
}
