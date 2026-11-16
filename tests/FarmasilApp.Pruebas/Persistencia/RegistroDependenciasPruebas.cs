using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.USR;
using FarmasilApp.Persistencia;
using FarmasilApp.Persistencia.INV;
using FarmasilApp.Persistencia.PAG;
using FarmasilApp.Persistencia.PRV;
using FarmasilApp.Persistencia.USR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.Persistencia;

public class RegistroDependenciasPruebas
{
    private static IConfiguration Configuracion(string? cadena)
    {
        var valores = new Dictionary<string, string?>();
        if (cadena is not null)
        {
            valores["ConnectionStrings:DB_FARMASIL"] = cadena;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(valores).Build();
    }

    [Fact]
    public void RegistraLasOpcionesConLaCadenaDeLaConfiguracion()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarPersistencia(Configuracion("Server=prueba;Database=DB_FARMASIL"));

        using var proveedor = servicios.BuildServiceProvider();
        var opciones = proveedor.GetRequiredService<OpcionesBaseDatos>();
        Assert.Equal("Server=prueba;Database=DB_FARMASIL", opciones.CadenaConexion);
    }

    [Fact]
    public void LasOpcionesSonUnaUnicaInstancia()
    {
        var servicios = new ServiceCollection();
        servicios.AgregarPersistencia(Configuracion("Server=prueba"));

        using var proveedor = servicios.BuildServiceProvider();

        Assert.Same(
            proveedor.GetRequiredService<OpcionesBaseDatos>(),
            proveedor.GetRequiredService<OpcionesBaseDatos>());
    }

    [Fact]
    public void SiFaltaLaCadenaFallaConUnMensajeQueNombraLaClave()
    {
        var servicios = new ServiceCollection();

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => servicios.AgregarPersistencia(Configuracion(null)));

        Assert.Contains("DB_FARMASIL", excepcion.Message);
        Assert.Contains("ConnectionStrings", excepcion.Message);
    }

    [Fact]
    public void SiLaCadenaEstaEnBlancoFalla()
    {
        var servicios = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => servicios.AgregarPersistencia(Configuracion("   ")));
    }

    [Fact]
    public void LaConfiguracionEsObligatoria()
    {
        var servicios = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() => servicios.AgregarPersistencia(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void LasOpcionesRechazanUnaCadenaVacia(string? cadena)
    {
        Assert.Throws<ArgumentException>(() => new OpcionesBaseDatos(cadena!));
    }

    [Fact]
    public void AgregarPersistenciaRegistraInventarioRepositorio()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarPersistencia(Configuracion("Server=prueba"));

        var registro = Assert.Single(
            servicios,
            servicio => servicio.ServiceType == typeof(IInventarioRepositorio));

        Assert.Equal(typeof(InventarioRepositorio), registro.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, registro.Lifetime);
    }

    [Fact]
    public void AgregarPersistenciaRegistraMetodosPagoRepositorio()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarPersistencia(Configuracion("Server=prueba"));

        var registro = Assert.Single(
            servicios,
            servicio => servicio.ServiceType == typeof(IMetodosPagoRepositorio));

        Assert.Equal(typeof(MetodosPagoRepositorio), registro.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, registro.Lifetime);
    }

    [Fact]
    public void AgregarPersistenciaRegistraProveedoresRepositorio()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarPersistencia(Configuracion("Server=prueba"));

        var registro = Assert.Single(
            servicios,
            servicio => servicio.ServiceType == typeof(IProveedoresRepositorio));

        Assert.Equal(typeof(ProveedoresRepositorio), registro.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, registro.Lifetime);
    }

    [Fact]
    public void AgregarPersistenciaRegistraUsuariosRepositorio()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarPersistencia(Configuracion("Server=prueba"));

        var registro = Assert.Single(
            servicios,
            servicio => servicio.ServiceType == typeof(IUsuariosRepositorio));

        Assert.Equal(typeof(UsuariosRepositorio), registro.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, registro.Lifetime);
    }
}
