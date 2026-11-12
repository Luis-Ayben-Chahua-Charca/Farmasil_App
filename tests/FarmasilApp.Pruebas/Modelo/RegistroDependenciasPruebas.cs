using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.Modelo;

public class RegistroDependenciasPruebas
{
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
}
