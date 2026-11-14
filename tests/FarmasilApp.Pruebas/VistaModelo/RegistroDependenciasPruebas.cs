using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Pruebas.INV;
using FarmasilApp.Pruebas.PRV;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PRV;
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
