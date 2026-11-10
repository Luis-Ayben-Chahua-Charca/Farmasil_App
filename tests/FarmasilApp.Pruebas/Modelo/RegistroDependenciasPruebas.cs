using FarmasilApp.Modelo.Comun;
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
    public void AgregarModeloTodaviaNoRegistraNingunServicio()
    {
        var servicios = new ServiceCollection();

        servicios.AgregarModelo();

        Assert.Empty(servicios);
    }

    [Fact]
    public void AgregarModeloSePuedeEncadenarConLasOtrasCapas()
    {
        var servicios = new ServiceCollection();

        var excepcion = Record.Exception(() => servicios.AgregarModelo().AgregarModelo());

        Assert.Null(excepcion);
    }
}
