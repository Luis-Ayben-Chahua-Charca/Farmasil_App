
using System.ComponentModel;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.ALV;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.DOC;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.PAG;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PRV;
using FarmasilApp.VistaModelo.USR;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.VistaModelo;

public class MainViewModelPruebas
{
    private static readonly string[] ModulosDelMenu =
    {
        "Comprobantes tributarios",
        "Inventario",
        "Alertas de vencimiento",
        "Métodos de pago",
        "Usuarios",
        "Proveedores"
    };

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void ElAdministradorPuedeAbrirTodosLosModulosDelMenu()
    {
        using var proveedor = CrearProveedor(Rol.Administrador, out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Equal(
            ModulosDelMenu,
            principal.Modulos.Select(modulo => modulo.Nombre));

        Assert.All(principal.Modulos, modulo =>
        {
            Assert.True(modulo.Habilitado);
            Assert.Null(modulo.Motivo);
        });
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void ElTecnicoVeTodosLosModulosPeroSoloPuedeAbrirLosDeSuRol()
    {
        using var proveedor = CrearProveedor(Rol.Tecnico, out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        // El técnico puede ver todos los módulos.
        Assert.Equal(
            ModulosDelMenu,
            principal.Modulos.Select(modulo => modulo.Nombre));

        // Solo puede abrir los módulos autorizados.
        Assert.Equal(
            new[]
            {
                "Comprobantes tributarios",
                "Inventario",
                "Alertas de vencimiento",
                "Métodos de pago"
            },
            principal.Modulos
                .Where(modulo => modulo.Habilitado)
                .Select(modulo => modulo.Nombre));
    }

    [Theory]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-016")]
    [InlineData("Usuarios")]
    [InlineData("Proveedores")]
    public void UnModuloNoPermitidoQuedaDeshabilitadoConElMotivoYNoNavega(
        string nombre)
    {
        using var proveedor = CrearProveedor(Rol.Tecnico, out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        var modulo = Assert.Single(
            principal.Modulos,
            opcion => opcion.Nombre == nombre);

        modulo.AbrirCommand.Execute(null);

        Assert.False(modulo.Habilitado);
        Assert.False(modulo.AbrirCommand.CanExecute(null));
        Assert.Equal(
            "Solo disponible para el Administrador.",
            modulo.Motivo);

        Assert.Equal(typeof(InicioViewModel), navegacion.Destino);
    }

    [Theory]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    [InlineData("Comprobantes tributarios", typeof(ComprobantesViewModel))]
    [InlineData("Alertas de vencimiento", typeof(AlertasViewModel))]
    [InlineData("Inventario", typeof(InventarioViewModel))]
    [InlineData("Métodos de pago", typeof(MetodosPagoViewModel))]
    [InlineData("Usuarios", typeof(UsuariosViewModel))]
    [InlineData("Proveedores", typeof(ProveedoresViewModel))]
    public void LaOpcionDeModuloNavegaASuVistaModelo(
        string nombre,
        Type destino)
    {
        using var proveedor = CrearProveedor(
            Rol.Administrador,
            out var navegacion);

        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Single(
            principal.Modulos,
            modulo => modulo.Nombre == nombre)
            .AbrirCommand.Execute(null);

        Assert.Equal(destino, navegacion.Destino);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void UnModuloRegistradoSeAgregaAlMenuYSeHabilitaSoloParaSusRoles()
    {
        using var proveedor = CrearProveedor(
            Rol.Tecnico,
            out _,
            servicios => servicios.AgregarModuloNavegable<PantallaPruebaViewModel>(
                "Módulo de prueba",
                Rol.Tecnico));

        var principal = proveedor.GetRequiredService<MainViewModel>();

        var modulo = Assert.Single(
            principal.Modulos,
            opcion => opcion.Nombre == "Módulo de prueba");

        Assert.True(modulo.Habilitado);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void UnModuloSinRolesNoSePuedeRegistrar()
    {
        Assert.Throws<ArgumentException>(
            () => new ModuloMenu("Sin roles", () => { }));
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

        public void NavegarA<TViewModel>()
            where TViewModel : ViewModelBase
        {
            Destino = typeof(TViewModel);

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(Actual)));
        }
    }
}
