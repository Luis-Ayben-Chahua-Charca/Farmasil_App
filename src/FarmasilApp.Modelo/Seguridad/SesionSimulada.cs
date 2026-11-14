using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Implementación simulada de <see cref="ISesion"/> mientras USR no esté listo (plan, sección 10).
/// La sesión es siempre la misma cuenta de los datos de prueba, <c>admin_prueba</c> (id 1, Administrador)
/// o <c>tecnico_prueba</c> (id 2, Tecnico), según el rol con que se construya: database/002_datos_prueba.sql.
/// No tiene estado: no hay inicio ni cierre de sesión.
/// </summary>
public sealed class SesionSimulada : ISesion
{
    public SesionSimulada(Rol rol = Rol.Administrador)
    {
        UsuarioActual = rol switch
        {
            Rol.Administrador => new SesionUsuario { IdUsuario = 1, NombreUsuario = "admin_prueba", Rol = Rol.Administrador },
            Rol.Tecnico => new SesionUsuario { IdUsuario = 2, NombreUsuario = "tecnico_prueba", Rol = Rol.Tecnico },
            _ => throw new ArgumentOutOfRangeException(nameof(rol), rol, "Rol desconocido."),
        };
    }

    public SesionUsuario UsuarioActual { get; }

    public void ExigirRol(Rol rol)
    {
        if (!Enum.IsDefined(rol))
        {
            throw new ArgumentOutOfRangeException(nameof(rol), rol, "Rol desconocido.");
        }

        if (UsuarioActual.Rol != rol)
        {
            throw new AccesoDenegadoException(
                $"La operación exige el rol {rol} y la sesión tiene el rol {UsuarioActual.Rol}.");
        }
    }
}
