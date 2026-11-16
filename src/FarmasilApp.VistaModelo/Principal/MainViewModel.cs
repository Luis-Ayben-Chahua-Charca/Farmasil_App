using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>VistaModelo de la ventana principal: contiene la pantalla activa y arranca en la de inicio.</summary>
public class MainViewModel : ViewModelBase
{
    public MainViewModel(
        INavegacionServicio navegacion,
        ISesion sesion,
        IEnumerable<ModuloMenuViewModel> modulosRegistrados)
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
            .Where(modulo => modulo.PuedeAbrirsePara(rol))
            .ToArray();
        Navegacion.NavegarA<InicioViewModel>();
    }

    public INavegacionServicio Navegacion { get; }

    public IReadOnlyList<ModuloMenuViewModel> Modulos { get; }

    public string Titulo => "FARMASIL";
}

public sealed class ModuloMenuViewModel
{
    private readonly HashSet<Rol> _rolesPermitidos;

    public ModuloMenuViewModel(
        string nombre,
        RelayCommand abrirCommand,
        params Rol[] rolesPermitidos)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        AbrirCommand = abrirCommand ?? throw new ArgumentNullException(nameof(abrirCommand));
        ArgumentNullException.ThrowIfNull(rolesPermitidos);
        if (rolesPermitidos.Length == 0 || rolesPermitidos.Any(rol => !Enum.IsDefined(rol)))
        {
            throw new ArgumentException("Se requieren roles válidos para el módulo.", nameof(rolesPermitidos));
        }

        _rolesPermitidos = new HashSet<Rol>(rolesPermitidos);
    }

    public string Nombre { get; }

    public RelayCommand AbrirCommand { get; }

    public bool PuedeAbrirsePara(Rol rol) => _rolesPermitidos.Contains(rol);
}
