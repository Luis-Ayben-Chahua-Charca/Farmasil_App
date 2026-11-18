using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>
/// VistaModelo de la ventana principal: contiene la pantalla activa, arma el menú para el rol de la sesión
/// y arranca en la pantalla de inicio.
/// </summary>
/// <remarks>
/// Es única en la aplicación y arma el menú una sola vez, con la sesión simulada. Cuando exista el inicio de
/// sesión (#88), el menú tendrá que rehacerse al iniciar y al cerrar sesión.
/// </remarks>
public class MainViewModel : ViewModelBase
{
    public MainViewModel(
        INavegacionServicio navegacion,
        ISesion sesion,
        IEnumerable<ModuloMenu> modulosRegistrados)
    {
        Navegacion = navegacion ?? throw new ArgumentNullException(nameof(navegacion));
        ArgumentNullException.ThrowIfNull(sesion);
        ArgumentNullException.ThrowIfNull(modulosRegistrados);

        var rol = sesion.UsuarioActual.Rol;
        if (!Enum.IsDefined(rol))
        {
            throw new ArgumentOutOfRangeException(nameof(sesion), rol, "Rol desconocido.");
        }

        Modulos = modulosRegistrados
            .Select(modulo => new ModuloMenuViewModel(modulo, rol))
            .ToArray();
        Navegacion.NavegarA<InicioViewModel>();
    }

    public INavegacionServicio Navegacion { get; }

    /// <summary>Todos los módulos registrados; los que el rol no puede abrir quedan deshabilitados (DEC-075).</summary>
    public IReadOnlyList<ModuloMenuViewModel> Modulos { get; }

    public string Titulo => "FARMASIL";
}
