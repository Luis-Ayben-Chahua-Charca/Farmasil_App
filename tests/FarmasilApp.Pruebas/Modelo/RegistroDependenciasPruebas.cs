using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.REA;
using FarmasilApp.Modelo.RES;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.Persistencia;
using FarmasilApp.Pruebas.INV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.Modelo;

public class RegistroDependenciasPruebas
{
    /// <summary>Las nueve interfaces entre módulos de DEC-069 y la clase simulada que las implementa mientras el módulo no esté listo.</summary>
    public static TheoryData<Type, Type> Contratos => new()
    {
        { typeof(ISesion), typeof(SesionSimulada) },
        { typeof(IPagos), typeof(PagosSimulado) },
        { typeof(IRestricciones), typeof(RestriccionesSimulado) },
        { typeof(IProveedores), typeof(ProveedoresSimulado) },
        { typeof(IInventario), typeof(InventarioSimulado) },
        { typeof(ILotes), typeof(LotesSimulado) },
        { typeof(IVentas), typeof(VentasSimulado) },
        { typeof(IDevoluciones), typeof(DevolucionesSimulado) },
        { typeof(IReabastecimiento), typeof(ReabastecimientoSimulado) },
    };

    private static ServiceProvider CrearProveedor()
    {
        var servicios = new ServiceCollection();
        servicios.AgregarModelo();
        // La Persistencia aporta el repositorio real; aquí basta uno de prueba para poder resolver InventarioServicio.
        servicios.AddSingleton<IInventarioRepositorio, RepositorioDeInventarioFalso>();
        return servicios.BuildServiceProvider();
    }

    [Fact]
    public void AgregarModeloDevuelveLaMismaColeccion()
    {
        var servicios = new ServiceCollection();

        var resultado = servicios.AgregarModelo();

        Assert.Same(servicios, resultado);
    }

    [Fact]
    public void AgregarModeloRegistraInventarioServicio()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarModelo();

        var registro = Assert.Single(
            servicios,
            servicio => servicio.ServiceType == typeof(InventarioServicio));

        Assert.Equal(ServiceLifetime.Transient, registro.Lifetime);
    }

    [Fact]
    public void AgregarModeloSePuedeEncadenarConLasOtrasCapas()
    {
        var servicios = new ServiceCollection();

        var excepcion = Record.Exception(() => servicios.AgregarModelo().AgregarModelo());

        Assert.Null(excepcion);
    }

    [Theory]
    [MemberData(nameof(Contratos))]
    public void CadaContratoSeRegistraUnaSolaVezConSuSimulada(Type contrato, Type simulada)
    {
        // Una sola línea por interfaz: cambiar la simulada por la real es cambiar el tipo de esa línea (plan, sección 10).
        var servicios = new ServiceCollection();

        servicios.AgregarModelo();

        var registro = Assert.Single(servicios, servicio => servicio.ServiceType == contrato);
        Assert.Equal(simulada, registro.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, registro.Lifetime);
    }

    [Theory]
    [MemberData(nameof(Contratos))]
    public void CadaContratoSeResuelveYEsUnaUnicaInstancia(Type contrato, Type simulada)
    {
        using var proveedor = CrearProveedor();

        var primera = proveedor.GetRequiredService(contrato);
        var segunda = proveedor.GetRequiredService(contrato);

        Assert.IsType(simulada, primera);
        Assert.Same(primera, segunda);
    }

    [Fact]
    public void LaSesionSimuladaRegistradaEsLaAdministradora()
    {
        using var proveedor = CrearProveedor();

        var sesion = proveedor.GetRequiredService<ISesion>();

        Assert.Equal(Rol.Administrador, sesion.UsuarioActual.Rol);
    }

    [Fact]
    public void LasSimuladasCompartenElRelojDelSistema()
    {
        using var proveedor = CrearProveedor();

        Assert.Same(TimeProvider.System, proveedor.GetRequiredService<TimeProvider>());
    }

    [Fact]
    public void InventarioServicioSeResuelveConIProveedores()
    {
        using var proveedor = CrearProveedor();

        var excepcion = Record.Exception(() => proveedor.GetRequiredService<InventarioServicio>());

        Assert.Null(excepcion);
    }

    [Fact]
    public void ElArranqueDeLaAplicacionResuelveTodasLasDependenciasDelModeloYLaPersistencia()
    {
        // Mismo orden que App.OnStartup (VP-00). ValidateOnBuild falla si a algún servicio le falta una dependencia.
        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DB_FARMASIL"] = "Server=prueba;Database=DB_FARMASIL" })
            .Build();
        var servicios = new ServiceCollection();
        servicios.AgregarPersistencia(configuracion);
        servicios.AgregarModelo();

        var excepcion = Record.Exception(() =>
        {
            using var proveedor = servicios.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            proveedor.GetRequiredService<InventarioServicio>();
        });

        Assert.Null(excepcion);
    }

    [Fact]
    public void RegistrarLaImplementacionRealDespuesDeAgregarModeloLaSustituye()
    {
        // El módulo dueño cambia la línea de su interfaz: la última registración gana.
        var servicios = new ServiceCollection();
        servicios.AgregarModelo();
        servicios.AddSingleton<IPagos, PagosDeOtroModulo>();
        using var proveedor = servicios.BuildServiceProvider();

        Assert.IsType<PagosDeOtroModulo>(proveedor.GetRequiredService<IPagos>());
    }

    private sealed class PagosDeOtroModulo : IPagos
    {
        public IReadOnlyList<MetodoPagoDto> MetodosActivos() => Array.Empty<MetodoPagoDto>();
    }
}
