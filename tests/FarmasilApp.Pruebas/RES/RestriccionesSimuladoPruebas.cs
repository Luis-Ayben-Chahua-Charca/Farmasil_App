using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.RES;

namespace FarmasilApp.Pruebas.RES;

public class RestriccionesSimuladoPruebas
{
    [Fact]
    public void ElParacetamolTieneUnaRestriccionInformativaActiva()
    {
        // database/002_datos_prueba.sql: producto 1, restricción 1, 'Gestante', Informativa, Activo.
        IRestricciones restricciones = new RestriccionesSimulado();

        var resultado = restricciones.RestriccionesActivas(1);

        var restriccion = Assert.Single(resultado);
        Assert.Equal(1, restriccion.IdRestriccion);
        Assert.Equal(1, restriccion.IdProducto);
        Assert.Equal("Gestante", restriccion.CondicionCliente);
        Assert.Equal(TipoRestriccion.Informativa, restriccion.Tipo);
        Assert.Equal("Consultar dosis recomendada antes de la venta.", restriccion.MotivoAdvertencia);
    }

    [Fact]
    public void LaAmoxicilinaTieneUnaRestriccionBloqueanteActiva()
    {
        // database/002_datos_prueba.sql: producto 3, restricción 2, 'Alergia a penicilinas', Bloqueante, Activo.
        var restriccion = Assert.Single(new RestriccionesSimulado().RestriccionesActivas(3));

        Assert.Equal(2, restriccion.IdRestriccion);
        Assert.Equal(TipoRestriccion.Bloqueante, restriccion.Tipo);
        Assert.Equal("No completar la venta si el cliente presenta alergia.", restriccion.MotivoAdvertencia);
    }

    [Fact]
    public void UnaRestriccionInactivaNoSeDevuelve()
    {
        // El ibuprofeno (producto 2) solo tiene la restricción 3, que está inactiva.
        Assert.Empty(new RestriccionesSimulado().RestriccionesActivas(2));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(99)]
    public void UnMedicamentoSinRestriccionesDevuelveUnaListaVacia(int idProducto)
    {
        Assert.Empty(new RestriccionesSimulado().RestriccionesActivas(idProducto));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void UnIdentificadorNoPositivoFalla(int idProducto)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RestriccionesSimulado().RestriccionesActivas(idProducto));
    }
}
