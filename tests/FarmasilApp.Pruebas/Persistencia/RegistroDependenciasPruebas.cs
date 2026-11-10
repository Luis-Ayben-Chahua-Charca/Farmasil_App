using FarmasilApp.Persistencia;
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
}
