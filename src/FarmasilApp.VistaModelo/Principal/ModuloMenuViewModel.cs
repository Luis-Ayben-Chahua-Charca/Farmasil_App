using FarmasilApp.Modelo.Comun;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>
/// Opción del menú principal para el rol de la sesión. Todos los módulos aparecen en el menú, pero solo se
/// pueden abrir con los roles que la matriz de permisos les concede. Para los demás la opción queda
/// deshabilitada y <see cref="Motivo"/> explica por qué (RNF-0006, condición 4; DEC-075).
/// </summary>
public sealed class ModuloMenuViewModel
{
    public ModuloMenuViewModel(ModuloMenu modulo, Rol rolDeLaSesion)
    {
        ArgumentNullException.ThrowIfNull(modulo);

        Nombre = modulo.Nombre;
        Habilitado = modulo.PuedeAbrirsePara(rolDeLaSesion);
        Motivo = Habilitado ? null : $"Solo disponible para {DescribirRoles(modulo.RolesPermitidos)}.";

        // El comando no se puede ejecutar si el rol no tiene permiso: así WPF deshabilita el botón.
        // Si aun así se invoca, no navega: la defensa no depende de la interfaz.
        AbrirCommand = new RelayCommand(
            () =>
            {
                if (Habilitado)
                {
                    modulo.Abrir();
                }
            },
            () => Habilitado);
    }

    public string Nombre { get; }

    public bool Habilitado { get; }

    /// <summary>Por qué el rol de la sesión no puede abrir el módulo; <c>null</c> si puede.</summary>
    public string? Motivo { get; }

    public RelayCommand AbrirCommand { get; }

    private static string DescribirRoles(IEnumerable<Rol> roles) =>
        string.Join(" y ", roles.Select(rol => rol switch
        {
            Rol.Administrador => "el Administrador",
            Rol.Tecnico => "el Técnico",
            _ => rol.ToString()
        }));
}
