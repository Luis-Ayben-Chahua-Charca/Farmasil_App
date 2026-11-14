using FarmasilApp.Modelo.PAG;

namespace FarmasilApp.Pruebas.PAG;

public class PagosSimuladoPruebas
{
    [Fact]
    public void LosMetodosActivosSonLosDeLosDatosDePrueba()
    {
        // database/002_datos_prueba.sql: Efectivo (1) y Yape (2) activos; Tarjeta (3) inactivo.
        IPagos pagos = new PagosSimulado();

        var metodos = pagos.MetodosActivos();

        Assert.Collection(
            metodos,
            efectivo =>
            {
                Assert.Equal(1, efectivo.IdMetodoPago);
                Assert.Equal("Efectivo", efectivo.Nombre);
            },
            yape =>
            {
                Assert.Equal(2, yape.IdMetodoPago);
                Assert.Equal("Yape", yape.Nombre);
            });
    }

    [Fact]
    public void ElMetodoInactivoNoSeListaAunqueExistaEnLaBase()
    {
        var metodos = new PagosSimulado().MetodosActivos();

        Assert.DoesNotContain(metodos, metodo => metodo.Nombre == "Tarjeta");
    }

    [Fact]
    public void CadaConsultaDevuelveLosMismosDatos()
    {
        var pagos = new PagosSimulado();

        Assert.Equal(
            pagos.MetodosActivos().Select(m => m.IdMetodoPago),
            pagos.MetodosActivos().Select(m => m.IdMetodoPago));
    }
}
