using FarmasilApp.Modelo.DEV;

namespace FarmasilApp.Pruebas.DEV;

/// <summary>
/// Los datos esperados son los de database/002_datos_prueba.sql: la orden 1 está 'Pendiente' (hace 2 días) y lleva 2 unidades de la
/// remesa PAR-002 (id 2); la orden 2 está 'Completada' y no cuenta.
/// </summary>
public class DevolucionesSimuladoPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 14);

    private static IDevoluciones Devoluciones() => new DevolucionesSimulado(new RelojFijo(Hoy));

    [Fact]
    public void LaRemesaPar002TieneUnaOrdenPendiente()
    {
        var orden = Assert.Single(Devoluciones().OrdenesPendientesDeRemesa(2));

        Assert.Equal(1, orden.IdDevolucion);
        Assert.Equal(Hoy.AddDays(-2), orden.FechaCreacion);
        Assert.Equal(2, orden.CantidadDevolver);
    }

    [Theory]
    [InlineData(1)] // PAR-001: nunca estuvo en una orden
    [InlineData(6)] // DIC-BLOQ-001: solo está en la orden completada
    [InlineData(99)]
    public void UnaRemesaSinOrdenesPendientesDevuelveUnaListaVacia(int idLote)
    {
        Assert.Empty(Devoluciones().OrdenesPendientesDeRemesa(idLote));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UnIdentificadorNoPositivoFalla(int idLote)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Devoluciones().OrdenesPendientesDeRemesa(idLote));
    }

    [Fact]
    public void ElRelojEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new DevolucionesSimulado(null!));
    }
}
