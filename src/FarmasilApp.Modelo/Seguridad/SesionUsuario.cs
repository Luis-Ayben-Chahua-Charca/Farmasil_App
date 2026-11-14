using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Usuario de la sesión activa, tal como lo ven los demás módulos (DEC-069).
/// No incluye la contraseña ni su hash: el hash no sale nunca de Usuarios (ESP-0034).
/// </summary>
public sealed class SesionUsuario
{
    public int IdUsuario { get; init; }

    public string NombreUsuario { get; init; } = string.Empty;

    public Rol Rol { get; init; }
}
