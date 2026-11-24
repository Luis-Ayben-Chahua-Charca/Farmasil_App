using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Pruebas.VistaModelo;
using FarmasilApp.VistaModelo.ALV;

namespace FarmasilApp.Pruebas.ALV;

public sealed class AlertasViewModelPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0015")]
    [Trait("Tecnica", "T-002")]
    public async Task AlSeleccionarUnRetiroElComboMuestraSuRemesaAunqueYaNoEsteDisponible()
    {
        var vistaModelo = await CrearConUnRetiroAsync();

        vistaModelo.AlertaSeleccionada = vistaModelo.Alertas[0];

        Assert.Equal(3, vistaModelo.LoteFichaSeleccionado?.IdLote);
        Assert.Contains(vistaModelo.LotesFicha, l => l.IdLote == 3);
        Assert.Equal("23/12/2026", vistaModelo.FechaFicha);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0015")]
    [Trait("Tecnica", "T-002")]
    public async Task AlDejarDeSeleccionarElRetiroSuRemesaYaNoSeOfreceParaRegistrar()
    {
        var vistaModelo = await CrearConUnRetiroAsync();
        vistaModelo.AlertaSeleccionada = vistaModelo.Alertas[0];

        vistaModelo.AlertaSeleccionada = null;

        Assert.DoesNotContain(vistaModelo.LotesFicha, l => l.IdLote == 3);
    }

    private static async Task<AlertasViewModel> CrearConUnRetiroAsync()
    {
        var sesion = new SesionSimulada(Rol.Administrador);
        var servicio = new AlertasServicio(
            new RepositorioConUnRetiro(), new LotesSinRemesas(), sesion, new RelojFijo(), new DevolucionesSinPendientes());
        var vistaModelo = new AlertasViewModel(servicio, new MensajeServicioFalso(), sesion);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        return vistaModelo;
    }

    private sealed class RelojFijo : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private sealed class DevolucionesSinPendientes : IDevoluciones
    {
        public IReadOnlyList<OrdenPendienteDto> OrdenesPendientesDeRemesa(int idLote) => Array.Empty<OrdenPendienteDto>();
    }

    /// <summary>Sin remesas: la del retiro ya está bloqueada, así que LOT no la ofrece como disponible.</summary>
    private sealed class LotesSinRemesas : ILotes
    {
        public IReadOnlyList<DatosRemesaDto> RemesasDisponibles() => Array.Empty<DatosRemesaDto>();

        public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote) => Array.Empty<DatosRemesaDto>();

        public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite) => Array.Empty<DatosRemesaDto>();

        public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor) => Array.Empty<DatosRemesaDto>();

        public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles() => throw new NotSupportedException();

        public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto) => throw new NotSupportedException();

        public void Descontar(int idLote, int cantidad) => throw new NotSupportedException();

        public void Restituir(int idLote, int cantidad) => throw new NotSupportedException();

        public void Bloquear(int idLote) => throw new NotSupportedException();

        public void Liberar(int idLote) => throw new NotSupportedException();
    }

    private sealed class RepositorioConUnRetiro : IAlertasRepositorio
    {
        public int ObtenerUmbralMesesActivo() => 2;

        public IReadOnlyList<AlertaConsulta> ConsultarRetiros() => new[]
        {
            new AlertaConsulta
            {
                IdRetiro = 7,
                IdLote = 3,
                Medicamento = "Paracetamol",
                NumeroLote = "PAR-003",
                FechaVencimiento = new DateOnly(2026, 12, 23),
                EsLoteDefectuoso = false,
                Origen = "Retirado"
            }
        };

        public T EjecutarTransaccion<T>(Func<IAlertasUnidadTrabajo, T> operacion) => throw new NotSupportedException();
    }
}
