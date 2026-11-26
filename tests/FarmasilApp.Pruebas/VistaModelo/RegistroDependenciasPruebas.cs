using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Pruebas.ALV;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.USR;
using FarmasilApp.Pruebas.INV;
using FarmasilApp.Pruebas.PAG;
using FarmasilApp.Pruebas.PRV;
using FarmasilApp.Pruebas.USR;
using FarmasilApp.VistaModelo.ALV;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.LOT;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PAG;
using FarmasilApp.VistaModelo.PRV;
using FarmasilApp.VistaModelo.USR;
using FarmasilApp.VistaModelo.VEN;
using Microsoft.Extensions.DependencyInjection;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Pruebas.DOC;
using FarmasilApp.VistaModelo.DOC;

namespace FarmasilApp.Pruebas.VistaModelo;

public class RegistroDependenciasPruebas
{
    private static ServiceProvider CrearProveedor()
    {
        var servicios = new ServiceCollection();
        servicios.AgregarModelo();
        servicios.AddSingleton<IProveedoresRepositorio, RepositorioDeProveedoresFalso>();
        servicios.AddSingleton<IUsuariosRepositorio, RepositorioDeUsuariosFalso>();
        servicios.AddSingleton<IAlertasRepositorio, AlertasRepositorioFalso>();
        servicios.AddSingleton<IInventarioRepositorio, RepositorioDeInventarioFalso>();
        servicios.AddSingleton<ILotesRepositorio, RepositorioLotesFalso>();
        servicios.AddTransient<LotesServicio>();

        // Repositorio falso del módulo DOC.
        servicios.AddSingleton<IComprobanteRepositorio, ComprobanteRepositorioFalso>();

        servicios.AddSingleton<IMetodosPagoRepositorio, RepositorioDeMetodosPagoFalso>();
        // La Vista registra el servicio real de mensajes al arrancar (App.xaml.cs); aquí basta uno de prueba.
        servicios.AddSingleton<IMensajeServicio, MensajeServicioFalso>();
        // La Vista registra el servicio real de PDF al arrancar (App.xaml.cs); aquí basta uno de prueba.
        servicios.AddSingleton<IPdfServicio, PdfServicioFalso>();
        servicios.AgregarVistaModelo();
        return servicios.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    [Fact]
    public void ElProveedorSeConstruyeConTodasLasDependenciasResueltas()
    {
        using var proveedor = CrearProveedor();

        Assert.NotNull(proveedor.GetRequiredService<MainViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<ComprobantesViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<INavegacionServicio>());
        Assert.NotNull(proveedor.GetRequiredService<InicioViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<InicioSesionViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<AlertasViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<InventarioViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<LotesViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<MetodosPagoViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<ProveedoresViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<UsuariosViewModel>());
        Assert.NotNull(proveedor.GetRequiredService<VentaViewModel>());
    }

    [Fact]
    public void CadaModuloNavegableSeRegistraUnaSolaVez()
    {
        using var proveedor = CrearProveedor();

        var modulos = proveedor.GetServices<ModuloMenu>().Select(modulo => modulo.Nombre).ToArray();

        Assert.Equal(
    new[]
    {
      "Ventas",
      "Comprobantes tributarios",
    "Inventario",
    "Lotes",
    "Alertas de vencimiento",
    "Métodos de pago",
    "Usuarios",
    "Proveedores"
    },
    modulos);
    }

    [Fact]
    public void LaVentanaPrincipalArrancaEnElInicioDeSesion()
    {
        using var proveedor = CrearProveedor();

        var principal = proveedor.GetRequiredService<MainViewModel>();

        Assert.IsType<InicioSesionViewModel>(principal.Navegacion.Actual);
        Assert.False(principal.HaySesion);
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

    private sealed class RepositorioLotesFalso : ILotesRepositorio
    {
        public IReadOnlyList<RemesaConsulta> Consultar(
            int? idProducto,
            string? numeroLote,
            EstadoLote? estado)
            => Array.Empty<RemesaConsulta>();

        public RemesaConsulta? ObtenerPorId(int idLote)
            => null;

        public bool ExisteNumeroLote(
            int idProducto,
            string numeroLote,
            int? excluirIdLote = null)
            => false;

        public int Crear(NuevaRemesa remesa)
            => 1;

        public bool TieneDetalleDevolucion(int idLote)
            => false;

        public int CantidadComprometidaPendiente(int idLote)
            => 0;

        public void Actualizar(
            int idLote,
            ActualizacionRemesa remesa,
            EstadoLote estadoNuevo)
        {
        }

        public bool TieneDevolucionPendiente(int idLote)
            => false;

        public ResultadoBajaRemesa DarDeBaja(int idLote)
            => ResultadoBajaRemesa.Eliminada;
    }
}
