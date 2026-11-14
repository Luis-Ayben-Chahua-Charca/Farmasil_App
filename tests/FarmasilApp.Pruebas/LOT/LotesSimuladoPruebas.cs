using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;

namespace FarmasilApp.Pruebas.LOT;

/// <summary>
/// Los datos esperados son los de database/002_datos_prueba.sql: seis remesas con ids 1 a 6 en una base recién creada,
/// con vencimientos relativos a la fecha de hoy (DATEADD(MONTH, n, hoy)).
/// </summary>
public class LotesSimuladoPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 14);

    private static ILotes Lotes(DateOnly? dia = null) => new LotesSimulado(new RelojFijo(dia ?? Hoy));

    // ---------------------------------------------------------------- MedicamentosVendibles

    [Fact]
    public void LosMedicamentosVendiblesSonParacetamolYAmoxicilina()
    {
        var medicamentos = Lotes().MedicamentosVendibles();

        Assert.Equal(new[] { 1, 3 }, medicamentos.Select(m => m.IdProducto));
    }

    [Fact]
    public void ElParacetamolTraeSusDatosYElStockDeSuUnicaRemesaVendible()
    {
        // PAR-001 (20 unidades) es vendible; PAR-002 (5) está bloqueada y no cuenta.
        var paracetamol = Lotes().MedicamentosVendibles().First();

        Assert.Equal("Paracetamol 500 mg", paracetamol.Nombre);
        Assert.Equal("Analgesico y antipiretico", paracetamol.AccionTerapeutica);
        Assert.Equal(2.50m, paracetamol.PrecioVenta);
        Assert.Equal(20, paracetamol.StockVendible);
    }

    [Fact]
    public void LaAmoxicilinaPorVencerSigueSiendoVendible()
    {
        var amoxicilina = Lotes().MedicamentosVendibles().Last();

        Assert.Equal("Amoxicilina 500 mg", amoxicilina.Nombre);
        Assert.Equal(12, amoxicilina.StockVendible);
    }

    [Fact]
    public void LaRemesaVencidaNoEsVendibleEnNingunDia()
    {
        // Los vencimientos de la simulada son relativos al reloj: IBU-VENC-001 siempre vence un mes antes de "hoy".
        // El límite exacto (vencimiento igual a la fecha de la venta) se prueba contra la base (DEC-033).
        foreach (var dia in new[] { Hoy, Hoy.AddDays(30), Hoy.AddYears(1) })
        {
            Assert.DoesNotContain(Lotes(dia).MedicamentosVendibles(), m => m.IdProducto == 2);
        }
    }

    // ---------------------------------------------------------------- SugerirFefo

    [Fact]
    public void LaRemesaSugeridaDelParacetamolEsPar001ConLosDatosDelMedicamento()
    {
        var remesa = Assert.Single(Lotes().SugerirFefo(1));

        Assert.Equal(1, remesa.IdLote);
        Assert.Equal(1, remesa.IdProducto);
        Assert.Equal("Paracetamol 500 mg", remesa.NombreMedicamento);
        Assert.Equal(2.50m, remesa.PrecioVenta);
        Assert.Equal("PAR-001", remesa.NumeroLote);
        Assert.Equal(Hoy.AddMonths(12), remesa.FechaVencimiento);
        Assert.Equal(20, remesa.StockActual);
    }

    [Theory]
    [InlineData(2)] // IBU-VENC-001: vencida
    [InlineData(4)] // LOR-AGOT-001: agotada
    [InlineData(5)] // DIC-BLOQ-001: bloqueada
    [InlineData(6)] // Omeprazol: sin remesas
    [InlineData(7)] // Producto descontinuado
    [InlineData(99)]
    public void UnMedicamentoSinRemesaVendibleNoTieneSugerencia(int idProducto)
    {
        Assert.Empty(Lotes().SugerirFefo(idProducto));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SugerirConUnIdentificadorNoPositivoFalla(int idProducto)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Lotes().SugerirFefo(idProducto));
    }

    // ---------------------------------------------------------------- DatosRemesas

    [Fact]
    public void LosDatosDeUnaRemesaIncluyenProveedorEstadoYStock()
    {
        var datos = Lotes().DatosRemesas(new[] { 2 }).Single();

        Assert.Equal(2, datos.IdLote);
        Assert.Equal("PAR-002", datos.NumeroLote);
        Assert.Equal(Hoy.AddMonths(10), datos.FechaVencimiento);
        Assert.Equal("Paracetamol 500 mg", datos.NombreMedicamento);
        Assert.Equal(1, datos.IdProveedor);
        Assert.Equal(EstadoLote.BloqueadoPorDevolucion, datos.EstadoLote);
        Assert.Equal(5, datos.StockActual);
    }

    [Fact]
    public void LosIdentificadoresQueNoExistenNoSeDevuelven()
    {
        var datos = Lotes().DatosRemesas(new[] { 1, 99, 5 });

        Assert.Equal(new[] { 1, 5 }, datos.Select(d => d.IdLote));
        Assert.Equal(EstadoLote.Agotado, datos[1].EstadoLote);
        Assert.Equal(0, datos[1].StockActual);
    }

    [Fact]
    public void SinIdentificadoresNoHayDatos()
    {
        Assert.Empty(Lotes().DatosRemesas(Array.Empty<int>()));
    }

    [Fact]
    public void LosIdentificadoresSonObligatorios()
    {
        Assert.Throws<ArgumentNullException>(() => Lotes().DatosRemesas(null!));
    }

    [Fact]
    public void UnIdentificadorNoPositivoEnLaListaFalla()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Lotes().DatosRemesas(new[] { 1, 0 }));
    }

    // ---------------------------------------------------------------- RemesasDevolvibles

    [Fact]
    public void LasRemesasDevolviblesDelProveedor1VanPorVencimientoYExcluyenLaAgotada()
    {
        // PAR-002 (bloqueada, vence en 10 meses) y PAR-001 (12 meses); LOR-AGOT-001 tiene stock 0.
        var remesas = Lotes().RemesasDevolvibles(1);

        Assert.Equal(new[] { 2, 1 }, remesas.Select(r => r.IdLote));
    }

    [Fact]
    public void LasRemesasDevolviblesDelProveedor3IncluyenLaBloqueada()
    {
        var remesas = Lotes().RemesasDevolvibles(3);

        Assert.Equal(new[] { 4, 6 }, remesas.Select(r => r.IdLote));
        Assert.Equal(EstadoLote.BloqueadoPorDevolucion, remesas[1].EstadoLote);
    }

    [Fact]
    public void UnaRemesaVencidaPeroDisponibleSigueSiendoDevolvible()
    {
        // ESP-0025 solo mira el estado y el stock: IBU-VENC-001 sigue 'Disponible'.
        var remesa = Assert.Single(Lotes().RemesasDevolvibles(2));

        Assert.Equal("IBU-VENC-001", remesa.NumeroLote);
    }

    [Theory]
    [InlineData(4)] // proveedor inactivo, sin remesas
    [InlineData(99)]
    public void UnProveedorSinRemesasDevuelveUnaListaVacia(int idProveedor)
    {
        Assert.Empty(Lotes().RemesasDevolvibles(idProveedor));
    }

    [Fact]
    public void DevolvibleConUnProveedorNoPositivoFalla()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Lotes().RemesasDevolvibles(0));
    }

    // ---------------------------------------------------------------- RemesasPorVencer

    [Fact]
    public void ConUmbralDeDosMesesVencenLaYaVencidaYLaProxima()
    {
        // ESP-0013: estado 'Disponible', stock > 0 y fecha_vencimiento <= hoy + umbral, sin cota inferior.
        var remesas = Lotes().RemesasPorVencer(Hoy.AddMonths(2));

        Assert.Equal(new[] { 3, 4 }, remesas.Select(r => r.IdLote));
    }

    [Fact]
    public void LaFechaLimiteEsInclusiva()
    {
        var vencimiento = Hoy.AddMonths(1);

        Assert.Contains(Lotes().RemesasPorVencer(vencimiento), r => r.IdLote == 4);
        Assert.DoesNotContain(Lotes().RemesasPorVencer(vencimiento.AddDays(-1)), r => r.IdLote == 4);
    }

    [Fact]
    public void UnaRemesaBloqueadaNoCuentaAunqueSuFechaLlegueAlLimite()
    {
        // PAR-002 y DIC-BLOQ-001 están bloqueadas: solo cuentan las 'Disponible'.
        var remesas = Lotes().RemesasPorVencer(Hoy.AddYears(5));

        Assert.DoesNotContain(remesas, r => r.EstadoLote != EstadoLote.Disponible);
        Assert.Equal(new[] { 3, 4, 1 }, remesas.Select(r => r.IdLote));
    }

    [Fact]
    public void SinRemesasPorVencerLaListaEstaVacia()
    {
        Assert.Empty(Lotes().RemesasPorVencer(Hoy.AddMonths(-2)));
    }

    // ---------------------------------------------------------------- RemesasDisponibles

    [Fact]
    public void LasRemesasDisponiblesTienenStockYEstanDisponiblesOBloqueadas()
    {
        // Vencimientos: IBU −1 mes, AMO +1, DIC +8, PAR-002 +10, PAR-001 +12. LOR-AGOT-001 (stock 0) no entra.
        var remesas = Lotes().RemesasDisponibles();

        Assert.Equal(new[] { 3, 4, 6, 2, 1 }, remesas.Select(r => r.IdLote));
    }

    // ---------------------------------------------------------------- Operaciones que escriben

    [Fact]
    public void DescontarNoBajaElStockDeLaSimulada()
    {
        var lotes = Lotes();

        lotes.Descontar(1, 5);

        Assert.Equal(20, lotes.DatosRemesas(new[] { 1 }).Single().StockActual);
    }

    [Fact]
    public void LasOperacionesQueEscribenConArgumentosValidosNoLanzan()
    {
        var lotes = Lotes();

        var excepcion = Record.Exception(() =>
        {
            lotes.Descontar(1, 1);
            lotes.Restituir(1, 1);
            lotes.Bloquear(1);
            lotes.Liberar(2);
        });

        Assert.Null(excepcion);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -4)]
    public void DescontarYRestituirRechazanIdentificadorOCantidadNoPositivos(int idLote, int cantidad)
    {
        var lotes = Lotes();

        Assert.Throws<ArgumentOutOfRangeException>(() => lotes.Descontar(idLote, cantidad));
        Assert.Throws<ArgumentOutOfRangeException>(() => lotes.Restituir(idLote, cantidad));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void BloquearYLiberarRechazanUnIdentificadorNoPositivo(int idLote)
    {
        var lotes = Lotes();

        Assert.Throws<ArgumentOutOfRangeException>(() => lotes.Bloquear(idLote));
        Assert.Throws<ArgumentOutOfRangeException>(() => lotes.Liberar(idLote));
    }

    [Fact]
    public void ElRelojEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new LotesSimulado(null!));
    }
}
