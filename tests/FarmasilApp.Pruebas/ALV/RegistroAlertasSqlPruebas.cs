using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Persistencia;
using FarmasilApp.Persistencia.ALV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.ALV;

public sealed class RegistroAlertasSqlPruebas
{
    [Fact]
    public void ArranqueResuelveElServicioAlvSinCambiarContratoGlobalDeLotes()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["ConnectionStrings:DB_FARMASIL"] = "Server=prueba;Database=DB_FARMASIL" })
            .Build();
        var servicios = new ServiceCollection();
        servicios.AgregarPersistencia(config);
        servicios.AgregarModelo();
        servicios.AgregarConsultaAlertasSql();

        using var proveedor = servicios.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });

        Assert.IsType<LotesSimulado>(proveedor.GetRequiredService<ILotes>());
        Assert.NotNull(proveedor.GetRequiredService<AlertasServicio>());
        Assert.Single(servicios, d => d.ServiceType == typeof(AlertasServicio));
    }
}
