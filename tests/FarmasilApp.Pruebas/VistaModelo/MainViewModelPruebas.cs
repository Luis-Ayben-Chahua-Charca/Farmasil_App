using System.ComponentModel;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.USR;
using FarmasilApp.Pruebas.USR;
using FarmasilApp.VistaModelo.ALV;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.DOC;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.LOT;
using FarmasilApp.VistaModelo.PAG;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PRV;
using FarmasilApp.VistaModelo.USR;
using FarmasilApp.VistaModelo.VEN;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.VistaModelo;

/// <summary>
/// Ventana principal con la sesión real (#88): arranca en el inicio de sesión, arma el menú del rol de la cuenta que
/// inicia sesión y vuelve al inicio de sesión al cerrarla.
/// </summary>
public class MainViewModelPruebas
{
    private const string Contrasena = "Farmasil2026!";

    private static readonly string[] ModulosDelMenu =
    {
        "Ventas",
        "Comprobantes tributarios",
        "Inventario",
        "Lotes",
        "Alertas de vencimiento",
        "Métodos de pago",
        "Usuarios",
        "Proveedores"
    };

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public async Task ElAvisoSeRecibeDesdeElEventoYPermiteNavegarALaConsulta()
    {
        using var proveedor = CrearProveedor(out var navegacion,
            servicios => servicios.AddSingleton<CanalAvisosVencimientos>());
        var principal = proveedor.GetRequiredService<MainViewModel>();
        var canal = proveedor.GetRequiredService<CanalAvisosVencimientos>();
        var aviso = new AvisoVencimientos(3, new DateOnly(2027, 1, 26),
            new DateTimeOffset(2026, 11, 26, 14, 0, 0, TimeSpan.Zero));

        canal.Publicar(aviso); // la evaluación puede terminar antes de iniciar sesión
        Assert.False(principal.HayAvisoVencimientos);
        Assert.False(principal.AbrirAvisoVencimientosCommand.CanExecute(null));

        await IniciarSesionAsync(principal, "tecnico");
        Assert.True(principal.HayAvisoVencimientos);
        Assert.Contains("3 remesas", principal.TextoAvisoVencimientos);
        Assert.True(principal.AbrirAvisoVencimientosCommand.CanExecute(null));
        principal.AbrirAvisoVencimientosCommand.Execute(null);
        Assert.Equal(typeof(AlertasViewModel), navegacion.Destino);

        principal.CerrarSesionCommand.Execute(null);
        Assert.False(principal.HayAvisoVencimientos);
        Assert.False(principal.AbrirAvisoVencimientosCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void AlArrancarMuestraElInicioDeSesionSinMenuNiSesion()
    {
        using var proveedor = CrearProveedor(out var navegacion);

        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.Equal(typeof(InicioSesionViewModel), navegacion.Destino);
        Assert.IsType<InicioSesionViewModel>(navegacion.Actual);
        Assert.False(principal.HaySesion);
        Assert.Empty(principal.Modulos);
        Assert.Equal(string.Empty, principal.UsuarioSesion);
        Assert.False(principal.CerrarSesionCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task ElAdministradorPuedeAbrirTodosLosModulosDelMenu()
    {
        using var proveedor = CrearProveedor(out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        await IniciarSesionAsync(principal, "admin");

        Assert.Equal(ModulosDelMenu, principal.Modulos.Select(modulo => modulo.Nombre));
        Assert.All(principal.Modulos, modulo =>
        {
            Assert.True(modulo.Habilitado);
            Assert.Null(modulo.Motivo);
        });
        Assert.Equal("admin · Administrador", principal.UsuarioSesion);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task ElTecnicoVeTodosLosModulosPeroSoloPuedeAbrirLosDeSuRol()
    {
        using var proveedor = CrearProveedor(out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();

        await IniciarSesionAsync(principal, "tecnico");

        // El técnico puede ver todos los módulos.
        Assert.Equal(ModulosDelMenu, principal.Modulos.Select(modulo => modulo.Nombre));

        // Solo puede abrir los módulos autorizados.
        Assert.Equal(
            new[]
            {
                "Ventas",
                "Comprobantes tributarios",
                "Inventario",
                "Lotes",
                "Alertas de vencimiento",
                "Métodos de pago"
            },
            principal.Modulos
                .Where(modulo => modulo.Habilitado)
                .Select(modulo => modulo.Nombre));
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task AlIniciarSesionPasaALaPantallaDeInicioYNotificaElMenu()
    {
        using var proveedor = CrearProveedor(out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();
        var cambios = new List<string?>();
        principal.PropertyChanged += (_, e) => cambios.Add(e.PropertyName);

        await IniciarSesionAsync(principal, "admin");

        Assert.Equal(typeof(InicioViewModel), navegacion.Destino);
        Assert.True(principal.HaySesion);
        Assert.True(principal.CerrarSesionCommand.CanExecute(null));
        Assert.Contains(nameof(MainViewModel.HaySesion), cambios);
        Assert.Contains(nameof(MainViewModel.Modulos), cambios);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task UnInicioRechazadoNoArmaElMenuYSeQuedaEnElInicioDeSesion()
    {
        using var proveedor = CrearProveedor(out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();
        var inicio = Assert.IsType<InicioSesionViewModel>(navegacion.Actual);
        inicio.NombreUsuario = "admin";
        inicio.Contrasena = "incorrecta";

        await inicio.IniciarSesionCommand.ExecuteAsync();

        Assert.Equal(MensajesSesion.CredencialesIncorrectas, inicio.Mensaje);
        Assert.False(principal.HaySesion);
        Assert.Empty(principal.Modulos);
        Assert.Equal(typeof(InicioSesionViewModel), navegacion.Destino);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task CerrarSesionBorraLaSesionVaciaElMenuYVuelveAlInicioDeSesion()
    {
        using var proveedor = CrearProveedor(out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();
        await IniciarSesionAsync(principal, "admin");

        principal.CerrarSesionCommand.Execute(null);

        Assert.False(principal.HaySesion);
        Assert.False(proveedor.GetRequiredService<IAutenticacion>().HaySesion);
        Assert.Throws<AccesoDenegadoException>(() => proveedor.GetRequiredService<ISesion>().UsuarioActual);
        Assert.Empty(principal.Modulos);
        Assert.Equal(string.Empty, principal.UsuarioSesion);
        var inicio = Assert.IsType<InicioSesionViewModel>(navegacion.Actual);
        Assert.Equal(string.Empty, inicio.NombreUsuario);
        Assert.False(principal.CerrarSesionCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task DespuesDeCerrarSesionOtraCuentaIniciaConSuPropioMenu()
    {
        using var proveedor = CrearProveedor(out _);
        var principal = proveedor.GetRequiredService<MainViewModel>();
        await IniciarSesionAsync(principal, "admin");
        principal.CerrarSesionCommand.Execute(null);

        await IniciarSesionAsync(principal, "tecnico");

        Assert.Equal("tecnico · Tecnico", principal.UsuarioSesion);
        Assert.Contains(principal.Modulos, modulo => modulo.Nombre == "Usuarios" && !modulo.Habilitado);
    }

    [Theory]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-016")]
    [InlineData("Usuarios")]
    [InlineData("Proveedores")]
    public async Task UnModuloNoPermitidoQuedaDeshabilitadoConElMotivoYNoNavega(string nombre)
    {
        using var proveedor = CrearProveedor(out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();
        await IniciarSesionAsync(principal, "tecnico");

        var modulo = Assert.Single(principal.Modulos, opcion => opcion.Nombre == nombre);

        modulo.AbrirCommand.Execute(null);

        Assert.False(modulo.Habilitado);
        Assert.False(modulo.AbrirCommand.CanExecute(null));
        Assert.Equal("Solo disponible para el Administrador.", modulo.Motivo);

        Assert.Equal(typeof(InicioViewModel), navegacion.Destino);
    }

    [Theory]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    [InlineData("Ventas", typeof(VentaViewModel))]
    [InlineData("Comprobantes tributarios", typeof(ComprobantesViewModel))]
    [InlineData("Alertas de vencimiento", typeof(AlertasViewModel))]
    [InlineData("Inventario", typeof(InventarioViewModel))]
    [InlineData("Lotes", typeof(LotesViewModel))]
    [InlineData("Métodos de pago", typeof(MetodosPagoViewModel))]
    [InlineData("Usuarios", typeof(UsuariosViewModel))]
    [InlineData("Proveedores", typeof(ProveedoresViewModel))]
    public async Task LaOpcionDeModuloNavegaASuVistaModelo(string nombre, Type destino)
    {
        using var proveedor = CrearProveedor(out var navegacion);
        var principal = proveedor.GetRequiredService<MainViewModel>();
        await IniciarSesionAsync(principal, "admin");

        Assert.Single(principal.Modulos, modulo => modulo.Nombre == nombre).AbrirCommand.Execute(null);

        Assert.Equal(destino, navegacion.Destino);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public async Task UnModuloRegistradoSeAgregaAlMenuYSeHabilitaSoloParaSusRoles()
    {
        using var proveedor = CrearProveedor(
            out _,
            servicios => servicios.AgregarModuloNavegable<PantallaPruebaViewModel>("Módulo de prueba", Rol.Tecnico));
        var principal = proveedor.GetRequiredService<MainViewModel>();

        await IniciarSesionAsync(principal, "tecnico");

        var modulo = Assert.Single(principal.Modulos, opcion => opcion.Nombre == "Módulo de prueba");
        Assert.True(modulo.Habilitado);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void UnModuloSinRolesNoSePuedeRegistrar()
    {
        Assert.Throws<ArgumentException>(() => new ModuloMenu("Sin roles", () => { }));
    }

    private static async Task IniciarSesionAsync(MainViewModel principal, string usuario)
    {
        var inicio = Assert.IsType<InicioSesionViewModel>(principal.Navegacion.Actual);
        inicio.NombreUsuario = usuario;
        inicio.Contrasena = Contrasena;

        await inicio.IniciarSesionCommand.ExecuteAsync();
    }

    private static ServiceProvider CrearProveedor(
        out NavegacionFalsa navegacion,
        Action<IServiceCollection>? configurar = null)
    {
        var servicios = new ServiceCollection();

        servicios.AgregarVistaModelo();

        // Dos cuentas activas, una por rol, con la contraseña de prueba; la sesión es la real.
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin", Rol.Administrador, EstadoRegistro.Activo, HashDeContrasenasFalso.HashDe(Contrasena));
        repositorio.Agregar("tecnico", Rol.Tecnico, EstadoRegistro.Activo, HashDeContrasenasFalso.HashDe(Contrasena));
        servicios.AddSingleton<IUsuariosRepositorio>(repositorio);
        servicios.AddSingleton<IHashContrasenas, HashDeContrasenasFalso>();
        servicios.AddSingleton<SesionActual>();
        servicios.AddSingleton<ISesion>(p => p.GetRequiredService<SesionActual>());
        servicios.AddSingleton<IAutenticacion>(p => p.GetRequiredService<SesionActual>());

        var fake = new NavegacionFalsa();
        navegacion = fake;
        servicios.AddSingleton<INavegacionServicio>(fake);

        configurar?.Invoke(servicios);

        var proveedor = servicios.BuildServiceProvider();
        fake.Proveedor = proveedor;
        return proveedor;
    }

    private sealed class PantallaPruebaViewModel : ViewModelBase
    {
    }

    /// <summary>
    /// Navegación de prueba: anota el destino y solo crea la pantalla de inicio de sesión, que es la única cuyo
    /// objeto necesita la ventana principal. Las demás pantallas dependen de repositorios y servicios que no hacen falta aquí.
    /// </summary>
    private sealed class NavegacionFalsa : INavegacionServicio
    {
        public IServiceProvider? Proveedor { get; set; }

        public ViewModelBase? Actual { get; private set; }

        public Type? Destino { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NavegarA<TViewModel>()
            where TViewModel : ViewModelBase
        {
            Destino = typeof(TViewModel);
            Actual = typeof(TViewModel) == typeof(InicioSesionViewModel)
                ? Proveedor?.GetRequiredService<TViewModel>()
                : null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Actual)));
        }
    }
}
