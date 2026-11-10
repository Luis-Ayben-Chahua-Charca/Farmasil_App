using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>VistaModelo de la ventana principal: contiene la pantalla activa y arranca en la de inicio.</summary>
public class MainViewModel : ViewModelBase
{
    public MainViewModel(INavegacionServicio navegacion)
    {
        Navegacion = navegacion ?? throw new ArgumentNullException(nameof(navegacion));
        Navegacion.NavegarA<InicioViewModel>();
    }

    public INavegacionServicio Navegacion { get; }

    public string Titulo => "FARMASIL";
}
