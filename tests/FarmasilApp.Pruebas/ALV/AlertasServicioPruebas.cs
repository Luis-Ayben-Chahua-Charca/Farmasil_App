using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.ALV;

public sealed class AlertasServicioPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 16);

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-002")]
    public void LaConsultaConsolidaRemesasPorVencerYRetirosRegistrados()
    {
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, "AMO-PROX", Hoy.AddMonths(1)));
        var repositorio = new AlertasRepositorioFalso();
        repositorio.Retiros.Add(new AlertaConsulta
        {
            IdLote = 2, Medicamento = "Paracetamol", NumeroLote = "PAR-002",
            FechaVencimiento = Hoy.AddMonths(5), EsLoteDefectuoso = true, Origen = "Retirado"
        });
        var resultado = Servicio(repositorio, lotes).Consultar();
        Assert.Equal(2, resultado.Alertas.Count);
        Assert.Equal(1, resultado.TotalProximasAVencer);
        Assert.Contains(resultado.Alertas, a => a.Origen == "Por vencer" && a.NumeroLote == "AMO-PROX");
        Assert.Contains(resultado.Alertas, a => a.Origen == "Retirado" && a.EsLoteDefectuoso == true);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-001")]
    public void ElLimiteIncluyeElDiaExactoYExcluyeElDiaSiguiente()
    {
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, "ANTES", Hoy.AddMonths(2).AddDays(-1)));
        lotes.Items.Add(Remesa(2, "JUSTO", Hoy.AddMonths(2)));
        lotes.Items.Add(Remesa(3, "DESPUES", Hoy.AddMonths(2).AddDays(1)));
        var resultado = Servicio(new AlertasRepositorioFalso(), lotes).Consultar();
        Assert.Equal(2, resultado.TotalProximasAVencer);
        Assert.DoesNotContain(resultado.Alertas, a => a.NumeroLote == "DESPUES");
        Assert.Contains(resultado.Alertas, a => a.NumeroLote == "JUSTO");
    }

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-006")]
    public void NoCuentaLotesBloqueadosNiAgotados()
    {
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, "DISPONIBLE", Hoy.AddDays(20)));
        lotes.Items.Add(Remesa(2, "BLOQUEADO", Hoy.AddDays(20), EstadoLote.BloqueadoPorDevolucion));
        lotes.Items.Add(Remesa(3, "AGOTADO", Hoy.AddDays(20), EstadoLote.Agotado, 0));
        var resultado = Servicio(new AlertasRepositorioFalso(), lotes).Consultar();
        Assert.Equal(1, resultado.TotalProximasAVencer);
        Assert.Single(resultado.Alertas);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-018")]
    public void UnCambioDelUmbralSeReflejaEnLaConsultaSinModificarCodigo()
    {
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, "UNO", Hoy.AddMonths(1)));
        lotes.Items.Add(Remesa(2, "TRES", Hoy.AddMonths(3)));
        var repositorio = new AlertasRepositorioFalso { Umbral = 2 };
        var servicio = Servicio(repositorio, lotes);
        Assert.Single(servicio.Consultar().Alertas);
        repositorio.Umbral = 3;
        Assert.Equal(2, servicio.Consultar().Alertas.Count);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-006")]
    public void CadaResultadoConservaElNombreYElNumeroDeRemesa()
    {
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(17, "LOTE-17", Hoy.AddMonths(1)));
        var fila = Assert.Single(Servicio(new AlertasRepositorioFalso(), lotes).Consultar().Alertas);
        Assert.Equal("Amoxicilina 500 mg", fila.Medicamento);
        Assert.Equal("LOTE-17", fila.NumeroLote);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-002")]
    public void LosFiltrosDeLaTarea86SeAplicanSoloALaVista()
    {
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, "UNO", Hoy.AddMonths(1)));
        lotes.Items.Add(Remesa(2, "DOS", Hoy.AddMonths(1)));
        var resultado = Servicio(new AlertasRepositorioFalso(), lotes).Consultar(1, "Por vencer");
        Assert.Single(resultado.Alertas);
        Assert.Equal(1, resultado.Alertas[0].IdLote);
        Assert.Equal(2, resultado.TotalProximasAVencer);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0013")]
    [Trait("Tecnica", "T-012")]
    public void ElTecnicoPuedeConsultarAlertas()
    {
        var servicio = Servicio(new AlertasRepositorioFalso(), new LotesFalsos(), Rol.Tecnico);
        Assert.Empty(servicio.Consultar().Alertas);
    }

    private static AlertasServicio Servicio(AlertasRepositorioFalso repositorio, LotesFalsos lotes,
        Rol rol = Rol.Administrador) => new(repositorio, lotes, new SesionSimulada(rol), new RelojFijo(), new DevolucionesSimulado(new RelojFijo()));

    private static DatosRemesaDto Remesa(int id, string numero, DateOnly vencimiento,
        EstadoLote estado = EstadoLote.Disponible, int stock = 10) => new()
    {
        IdLote = id, NumeroLote = numero, FechaVencimiento = vencimiento,
        EstadoLote = estado, StockActual = stock, NombreMedicamento = "Amoxicilina 500 mg"
    };

    private sealed class RelojFijo : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 16, 12, 0, 0, TimeSpan.Zero);
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private sealed class LotesFalsos : ILotes
    {
        public List<DatosRemesaDto> Items { get; } = new();
        public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite) =>
            Items.Where(r => r.EstadoLote == EstadoLote.Disponible && r.StockActual > 0
                && r.FechaVencimiento <= fechaLimite).ToArray();
        public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles() => throw new NotSupportedException();
        public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto) => throw new NotSupportedException();
        public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote) => throw new NotSupportedException();
        public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor) => throw new NotSupportedException();
        public IReadOnlyList<DatosRemesaDto> RemesasDisponibles() => throw new NotSupportedException();
        public void Descontar(int idLote, int cantidad) => throw new NotSupportedException();
        public void Restituir(int idLote, int cantidad) => throw new NotSupportedException();
        public void Bloquear(int idLote) => throw new NotSupportedException();
        public void Liberar(int idLote) => throw new NotSupportedException();
    }
}
