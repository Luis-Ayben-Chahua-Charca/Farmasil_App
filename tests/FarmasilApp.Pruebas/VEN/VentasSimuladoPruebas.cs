using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.VEN;

/// <summary>
/// Los datos esperados son los de database/002_datos_prueba.sql: tres ventas con ids 1 a 3, hechas 10, 40 y 5 días antes de hoy.
/// </summary>
public class VentasSimuladoPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 14);

    private static IVentas Ventas() => new VentasSimulado(new RelojFijo(Hoy));

    [Fact]
    public void SinCriteriosSeDevuelvenLasTresVentas()
    {
        var ventas = Ventas().ConsultarVentas(new FiltroVentas());

        Assert.Equal(new[] { 1, 2, 3 }, ventas.Select(v => v.IdVenta));
    }

    [Fact]
    public void LaPrimeraVentaTraeSuCabeceraYSuDetalle()
    {
        var venta = Ventas().ConsultarVentas(new FiltroVentas()).First();

        Assert.Equal(new DateTime(2026, 11, 4, 12, 0, 0), venta.FechaHora);
        Assert.Equal(5.00m, venta.MontoTotal);
        Assert.Equal(1, venta.IdMetodoPago);
        Assert.Equal(2, venta.IdUsuario);
        var linea = Assert.Single(venta.Detalle);
        Assert.Equal(1, linea.IdLote);
        Assert.Equal(2, linea.Cantidad);
        Assert.Equal(2.50m, linea.PrecioUnitario);
    }

    [Fact]
    public void LaVentaTresFueConYapeYRegistradaPorLaAdministradora()
    {
        var venta = Ventas().ObtenerVenta(3);

        Assert.NotNull(venta);
        Assert.Equal(2, venta.IdMetodoPago);
        Assert.Equal(1, venta.IdUsuario);
        Assert.Equal(2.50m, venta.MontoTotal);
    }

    [Fact]
    public void ElMontoTotalEsLaSumaDeLasLineasDeCadaVenta()
    {
        foreach (var venta in Ventas().ConsultarVentas(new FiltroVentas()))
        {
            Assert.Equal(venta.MontoTotal, venta.Detalle.Sum(l => l.Cantidad * l.PrecioUnitario));
        }
    }

    [Fact]
    public void FiltrarPorUnDiaDevuelveSoloLasVentasDeEseDia()
    {
        var dia = Hoy.AddDays(-10);

        var ventas = Ventas().ConsultarVentas(new FiltroVentas { Desde = dia, Hasta = dia });

        Assert.Equal(new[] { 1 }, ventas.Select(v => v.IdVenta));
    }

    [Fact]
    public void LosLimitesDelRangoDeFechasSonInclusivos()
    {
        var ventas = Ventas().ConsultarVentas(new FiltroVentas { Desde = Hoy.AddDays(-10), Hasta = Hoy.AddDays(-5) });

        Assert.Equal(new[] { 1, 3 }, ventas.Select(v => v.IdVenta));
    }

    [Fact]
    public void SoloConDesdeSeIncluyenLasVentasPosteriores()
    {
        var ventas = Ventas().ConsultarVentas(new FiltroVentas { Desde = Hoy.AddDays(-9) });

        Assert.Equal(new[] { 3 }, ventas.Select(v => v.IdVenta));
    }

    [Fact]
    public void SoloConHastaSeIncluyenLasVentasAnteriores()
    {
        var ventas = Ventas().ConsultarVentas(new FiltroVentas { Hasta = Hoy.AddDays(-11) });

        Assert.Equal(new[] { 2 }, ventas.Select(v => v.IdVenta));
    }

    [Fact]
    public void FiltrarPorMedicamentoDevuelveLasVentasQueLoIncluyen()
    {
        // Paracetamol (producto 1): remesa PAR-001 en las ventas 1 y 3. Ibuprofeno (producto 2): IBU-VENC-001 en la venta 2.
        Assert.Equal(new[] { 1, 3 }, Ventas().ConsultarVentas(new FiltroVentas { IdProducto = 1 }).Select(v => v.IdVenta));
        Assert.Equal(new[] { 2 }, Ventas().ConsultarVentas(new FiltroVentas { IdProducto = 2 }).Select(v => v.IdVenta));
    }

    [Fact]
    public void LosCriteriosSeCombinan()
    {
        var ventas = Ventas().ConsultarVentas(new FiltroVentas { Desde = Hoy.AddDays(-6), IdProducto = 1 });

        Assert.Equal(new[] { 3 }, ventas.Select(v => v.IdVenta));
    }

    [Fact]
    public void UnMedicamentoSinVentasDevuelveUnaListaVacia()
    {
        Assert.Empty(Ventas().ConsultarVentas(new FiltroVentas { IdProducto = 99 }));
    }

    [Fact]
    public void ElFiltroEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => Ventas().ConsultarVentas(null!));
    }

    [Fact]
    public void UnRangoInvertidoFalla()
    {
        var filtro = new FiltroVentas { Desde = Hoy, Hasta = Hoy.AddDays(-1) };

        Assert.Throws<ArgumentException>(() => Ventas().ConsultarVentas(filtro));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UnMedicamentoNoPositivoEnElFiltroFalla(int idProducto)
    {
        var filtro = new FiltroVentas { IdProducto = idProducto };

        Assert.Throws<ArgumentOutOfRangeException>(() => Ventas().ConsultarVentas(filtro));
    }

    [Fact]
    public void ObtenerUnaVentaQueNoExisteDevuelveNulo()
    {
        Assert.Null(Ventas().ObtenerVenta(99));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ObtenerConUnIdentificadorNoPositivoFalla(int idVenta)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Ventas().ObtenerVenta(idVenta));
    }

    [Fact]
    public void ElRelojEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new VentasSimulado(null!));
    }
}
