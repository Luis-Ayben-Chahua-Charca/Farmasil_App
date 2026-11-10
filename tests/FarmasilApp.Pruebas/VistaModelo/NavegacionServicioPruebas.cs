using FarmasilApp.VistaModelo.Comun;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Pruebas.VistaModelo;

public class NavegacionServicioPruebas
{
    private class PantallaA : ViewModelBase
    {
    }

    private class PantallaB : ViewModelBase
    {
    }

    private class PantallaSinRegistrar : ViewModelBase
    {
    }

    private static NavegacionServicio CrearServicio()
    {
        var servicios = new ServiceCollection();
        servicios.AddTransient<PantallaA>();
        servicios.AddTransient<PantallaB>();
        return new NavegacionServicio(servicios.BuildServiceProvider());
    }

    [Fact]
    public void AlInicioNoHayPantalla()
    {
        Assert.Null(CrearServicio().Actual);
    }

    [Fact]
    public void NavegarAMuestraLaPantallaPedida()
    {
        var navegacion = CrearServicio();

        navegacion.NavegarA<PantallaA>();

        Assert.IsType<PantallaA>(navegacion.Actual);
    }

    [Fact]
    public void NavegarAOtraPantallaReemplazaLaActual()
    {
        var navegacion = CrearServicio();
        navegacion.NavegarA<PantallaA>();

        navegacion.NavegarA<PantallaB>();

        Assert.IsType<PantallaB>(navegacion.Actual);
    }

    [Fact]
    public void NavegarNotificaElCambioDeActual()
    {
        var navegacion = CrearServicio();
        var notificadas = new List<string?>();
        navegacion.PropertyChanged += (_, e) => notificadas.Add(e.PropertyName);

        navegacion.NavegarA<PantallaA>();

        Assert.Equal(new[] { nameof(INavegacionServicio.Actual) }, notificadas);
    }

    [Fact]
    public void NavegarAUnaPantallaNoRegistradaFalla()
    {
        var navegacion = CrearServicio();

        Assert.Throws<InvalidOperationException>(() => navegacion.NavegarA<PantallaSinRegistrar>());
    }

    [Fact]
    public void ElProveedorEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new NavegacionServicio(null!));
    }
}
