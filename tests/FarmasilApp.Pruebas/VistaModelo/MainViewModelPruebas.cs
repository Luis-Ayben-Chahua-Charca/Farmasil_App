using System.ComponentModel;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PRV;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.VistaModelo;

public class MainViewModelPruebas
{
    [Fact]
    public void ElAdministradorVeTodosLosModulosImplementadosYPermitidos()
    {
        using var proveedor = CrearProveedor(Rol.Administrador, out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Equal(new[] { "Inventario", "Proveedores" }, principal.Modulos.Select(modulo => modulo.Nombre));
    }

    [Fact]
    public void ElTecnicoSoloVeLosModulosPermitidosParaSuRol()
    {
        using var proveedor = CrearProveedor(Rol.Tecnico, out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Equal(new[] { "Inventario" }, principal.Modulos.Select(modulo => modulo.Nombre));
    }

    [Theory]
    [InlineData("Inventario", typeof(InventarioViewModel))]
    [InlineData("Proveedores", typeof(ProveedoresViewModel))]
    public void LaOpcionDeModuloNavegaASuVistaModelo(string nombre, Type destino)
    {
        using var proveedor = CrearProveedor(Rol.Administrador, out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Single(principal.Modulos, modulo => modulo.Nombre == nombre)
            .AbrirCommand.Execute(null);

        Assert.Equal(destino, navegacion.Destino);
    }

    [Fact]
    public void UnModuloRegistradoConUnPermisoSeAgregaAlMenuDeEseRol()
    {
        using var proveedor = CrearProveedor(
            Rol.Administrador,
            out _,
            servicios => servicios.AgregarModuloNavegable<PantallaPruebaViewModel>(
                "Módulo de prueba",
                Rol.Administrador));
        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Contains(principal.Modulos, modulo => modulo.Nombre == "Módulo de prueba");
    }

    private static ServiceProvider CrearProveedor(
        Rol rol,
        out NavegacionFalsa navegacion,
        Action<IServiceCollection>? configurar = null)
    {
        var servicios = new ServiceCollection();
        servicios.AgregarVistaModelo();
        navegacion = new NavegacionFalsa();
        servicios.AddSingleton<ISesion>(new SesionSimulada(rol));
        servicios.AddSingleton<INavegacionServicio>(navegacion);
        configurar?.Invoke(servicios);
        return servicios.BuildServiceProvider();
    }

    private sealed class PantallaPruebaViewModel : ViewModelBase
    {
    }

    private sealed class NavegacionFalsa : INavegacionServicio
    {
        public ViewModelBase? Actual { get; private set; }

        public Type? Destino { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NavegarA<TViewModel>() where TViewModel : ViewModelBase
        {
            Destino = typeof(TViewModel);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Actual)));
        }
    }
}
