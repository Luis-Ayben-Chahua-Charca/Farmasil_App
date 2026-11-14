using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.INV;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>VistaModelo de la ventana principal: contiene la pantalla activa y arranca en la de inicio.</summary>
public class MainViewModel : ViewModelBase
{
    public MainViewModel(INavegacionServicio navegacion, ISesion sesion)
    {
        Navegacion = navegacion ?? throw new ArgumentNullException(nameof(navegacion));
        ArgumentNullException.ThrowIfNull(sesion);

        Modulos = CrearMenu(sesion.UsuarioActual.Rol, Navegacion);
        Navegacion.NavegarA<InicioViewModel>();
    }

    public INavegacionServicio Navegacion { get; }

    public IReadOnlyList<ModuloMenuViewModel> Modulos { get; }

    public string Titulo => "FARMASIL";

    private static IReadOnlyList<ModuloMenuViewModel> CrearMenu(
        Rol rol,
        INavegacionServicio navegacion)
    {
        switch (rol)
        {
            case Rol.Administrador:
            case Rol.Tecnico:
                return new[]
                {
                    new ModuloMenuViewModel(
                        "Inventario",
                        new RelayCommand(() => navegacion.NavegarA<InventarioViewModel>())),
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(rol), rol, "Rol desconocido.");
        }
    }
}

public sealed class ModuloMenuViewModel
{
    public ModuloMenuViewModel(string nombre, RelayCommand abrirCommand)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        AbrirCommand = abrirCommand ?? throw new ArgumentNullException(nameof(abrirCommand));
    }

    public string Nombre { get; }

    public RelayCommand AbrirCommand { get; }
}
