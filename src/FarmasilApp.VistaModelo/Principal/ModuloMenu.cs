using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>
/// Módulo registrado para el menú principal: su nombre, cómo se abre y los roles que la matriz de permisos
/// (<c>diseno/07-matriz-permisos.md</c>) le concede para abrirlo. Se registra con
/// <c>AgregarModuloNavegable</c>; la ventana principal arma con él una <see cref="ModuloMenuViewModel"/>
/// para el rol de la sesión.
/// </summary>
public sealed class ModuloMenu
{
    private readonly Rol[] _rolesPermitidos;

    public ModuloMenu(string nombre, Action abrir, params Rol[] rolesPermitidos)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentNullException.ThrowIfNull(abrir);
        ArgumentNullException.ThrowIfNull(rolesPermitidos);

        if (rolesPermitidos.Length == 0 || rolesPermitidos.Any(rol => !Enum.IsDefined(rol)))
        {
            throw new ArgumentException("Se requieren roles válidos para el módulo.", nameof(rolesPermitidos));
        }

        Nombre = nombre;
        Abrir = abrir;
        _rolesPermitidos = rolesPermitidos.Distinct().ToArray();
    }

    public string Nombre { get; }

    /// <summary>Navega a la pantalla del módulo.</summary>
    public Action Abrir { get; }

    /// <summary>Roles que pueden abrir el módulo, en el orden en que se registraron.</summary>
    public IReadOnlyList<Rol> RolesPermitidos => _rolesPermitidos;

    public bool PuedeAbrirsePara(Rol rol) => _rolesPermitidos.Contains(rol);
}
