using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.USR;
using FarmasilApp.Pruebas.INV;
using FarmasilApp.Pruebas.PRV;
using FarmasilApp.Pruebas.USR;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PRV;
using FarmasilApp.VistaModelo.USR;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.VistaModelo;

public class RegistroDependenciasPruebas
{
    private static ServiceProvider CrearProveedor()
    {
        var servicios = new ServiceCollection();
        servicios.AgregarModelo();
        servicios.AddSingleton<IInventarioRepositorio, RepositorioDeInventarioFalso>();
        servicios.AddSingleton<IProveedoresRepositorio, RepositorioDeProveedoresFalso>();
        servicios.AddSingleton<IUsuariosRepositorio, RepositorioDeUsuariosFalso>();
        // La Vista registra el servicio real de mensajes al arrancar (App.xaml.cs); aquí basta uno de prueba.
        servicios.AddSingleton<IMensajeServicio, MensajeServicioFalso>();
        servicios.AgregarVistaModelo();
        return servicios.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    [Fact]
    public void ElProveedorSeConstruyeConTodasLasDependenciasResueltas()
    {
        using var proveedor = CrearProveedor();

        Assert.NotNull(proveedor.GetRequiredService<MainViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<INavegacionServicio>());
        Assert.NotNull(proveedor.GetRequiredService<InicioViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<ProveedoresViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<UsuariosViewModel>());
    }

    [Fact]
    public void LaVentanaPrincipalArrancaEnLaPantallaDeInicio()
    {
        using var proveedor = CrearProveedor();

        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.IsType<InicioViewModel>(principal.Navegacion.Actual);
    }

    [Fact]
    public void LaNavegacionEsUnaUnicaInstancia()
    {
        using var proveedor = CrearProveedor();

        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Same(proveedor.GetRequiredService<INavegacionServicio>(), principal.Navegacion);
    }

    [Fact]
    public void CadaPantallaSeCreaNuevaAlNavegar()
    {
        using var proveedor = CrearProveedor();

        var primera = proveedor.GetRequiredService<InicioViewModel>();
        var segunda = proveedor.GetRequiredService<InicioViewModel>();

        Assert.NotSame(primera, segunda);
    }
}
