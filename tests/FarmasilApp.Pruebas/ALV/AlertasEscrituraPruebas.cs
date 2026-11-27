using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.ALV;

public sealed class AlertasEscrituraPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 23);

    [Fact]
    [Trait("Especificacion", "ESP-0014")]
    [Trait("Tecnica", "T-002")]
    public void LaTecnicaRegistraUnRetiroConCopiasHistoricasFechaYOrdenDeBloqueo()
    {
        var r = new RepositorioFalso();
        var lotes = new LotesFalso();
        lotes.Items.Add(Remesa(3, Hoy.AddDays(20)));
        var servicio = Servicio(r, lotes, rol: Rol.Tecnico);
        var id = servicio.Registrar(3, true, "DIG-101");
        var registro = Assert.Single(r.Retiros);
        Assert.Equal(id, registro.IdRetiro);
        Assert.Equal(3, registro.IdLote);
        Assert.Equal("Paracetamol", registro.NombreProducto);
        Assert.Equal("PAR-003", registro.NumeroLote);
        Assert.Equal(Hoy.AddDays(20), registro.FechaVencimiento);
        Assert.Equal("DIG-101", registro.AlertaDigemid);
        Assert.True(registro.EsLoteDefectuoso);
        Assert.Equal(new[] { 3 }, lotes.Bloqueados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0014")]
    [Trait("Tecnica", "T-012")]
    public void SiElBloqueoFallaLaInsercionSeRevierteEnElRepositorioFalso()
    {
        var r = new RepositorioFalso();
        var lotes = new LotesFalso { FallarAlBloquear = true };
        lotes.Items.Add(Remesa(3, Hoy.AddDays(20)));
        Assert.Throws<InvalidOperationException>(() => Servicio(r, lotes).Registrar(3, false, null));
        Assert.Empty(r.Retiros);
        // Esto NO sustituye la prueba SQL de transacción compartida del PEN-039 (#99/#101).
    }

    [Fact]
    [Trait("Especificacion", "ESP-0014")]
    [Trait("Tecnica", "T-006")]
    public void RegistrarUnaRemesaNoBloqueaLasDemas()
    {
        var r = new RepositorioFalso();
        var lotes = new LotesFalso();
        lotes.Items.Add(Remesa(3, Hoy.AddDays(20)));
        lotes.Items.Add(Remesa(4, Hoy.AddDays(40)));
        Servicio(r, lotes).Registrar(3, false, null);
        Assert.Equal(new[] { 3 }, lotes.Bloqueados);
        Assert.DoesNotContain(4, lotes.Bloqueados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0015")]
    [Trait("Tecnica", "T-002")]
    public void ModificarSoloCambiaDefectoYDigemidSinLlamarALotes()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(1, 3, Hoy.AddDays(20)));
        var lotes = new LotesFalso();
        Servicio(r, lotes).Modificar(1, true, " DIG-205 ");
        var retiro = Assert.Single(r.Retiros);
        Assert.True(retiro.EsLoteDefectuoso);
        Assert.Equal("DIG-205", retiro.AlertaDigemid);
        Assert.Equal(3, retiro.IdLote);
        Assert.Equal("Paracetamol", retiro.NombreProducto);
        Assert.Equal("PAR-003", retiro.NumeroLote);
        Assert.Empty(lotes.Liberados);
        Assert.Empty(lotes.Bloqueados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0015")]
    [Trait("Tecnica", "T-012")]
    public void LaTecnicaNoPuedeModificarNiEliminar()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(1, 3, Hoy.AddDays(20)));
        var servicio = Servicio(r, new LotesFalso(), rol: Rol.Tecnico);
        Assert.Throws<AccesoDenegadoException>(() => servicio.Modificar(1, true, null));
        Assert.Throws<AccesoDenegadoException>(() => servicio.Eliminar(1));
        Assert.Single(r.Retiros);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0016")]
    [Trait("Tecnica", "T-006")]
    public void EliminarUnoDeDosRetirosMantieneLaRemesaBloqueada()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(1, 3, Hoy.AddDays(20)));
        r.Retiros.Add(Retiro(2, 3, Hoy.AddDays(20)));
        var lotes = new LotesFalso();
        lotes.Items.Add(Remesa(3, Hoy.AddDays(20)));
        var texto = Servicio(r, lotes).Eliminar(1);
        Assert.Equal(MensajesAlertas.EliminadaConOtrosRetiros, texto);
        Assert.Single(r.Retiros);
        Assert.Empty(lotes.Liberados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0016")]
    [Trait("Tecnica", "T-002")]
    public void ElUltimoRetiroSeEliminaYSolicitaLaLiberacion()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(1, 3, Hoy.AddDays(20)));
        var lotes = new LotesFalso();
        lotes.Items.Add(Remesa(3, Hoy.AddDays(20)));
        var texto = Servicio(r, lotes).Eliminar(1);
        Assert.Equal(MensajesAlertas.Eliminada, texto);
        Assert.Empty(r.Retiros);
        Assert.Equal(new[] { 3 }, lotes.Liberados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0016")]
    [Trait("Tecnica", "T-003")]
    public void UnRetiroHistoricoSinIdLoteSeBorraSinLiberar()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(2, null, Hoy.AddDays(-90)));
        var lotes = new LotesFalso();
        Assert.Equal(MensajesAlertas.Eliminada, Servicio(r, lotes).Eliminar(2));
        Assert.Empty(lotes.Liberados);
        Assert.Empty(r.Retiros);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0016")]
    [Trait("Tecnica", "T-003")]
    public void UnaRemesaVencidaNoPermiteLaEliminacion()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(1, 3, Hoy));
        var lotes = new LotesFalso();
        lotes.Items.Add(Remesa(3, Hoy));
        var error = Assert.Throws<ReglaDeNegocioException>(() => Servicio(r, lotes).Eliminar(1));
        Assert.Equal(MensajesAlertas.RemesaVencida, error.Message);
        Assert.Single(r.Retiros);
        Assert.Empty(lotes.Liberados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0016")]
    [Trait("Tecnica", "T-003")]
    public void LaDevolucionPendienteSeRechazaAntesDeLaFecha()
    {
        var r = new RepositorioFalso();
        r.Retiros.Add(Retiro(1, 2, Hoy));
        var lotes = new LotesFalso();
        var dev = new DevolucionesFalso { Pendiente = true };
        var error = Assert.Throws<ReglaDeNegocioException>(() => Servicio(r, lotes, dev).Eliminar(1));
        Assert.Equal(MensajesAlertas.DevolucionPendiente, error.Message);
        Assert.Single(r.Retiros);
    }

    private static AlertasServicio Servicio(RepositorioFalso r, LotesFalso lotes,
        DevolucionesFalso? devoluciones = null, Rol rol = Rol.Administrador)
        => new(r, lotes, new SesionSimulada(rol), new RelojFijo(), devoluciones ?? new DevolucionesFalso());

    private static DatosRemesaDto Remesa(int id, DateOnly vence) => new()
    {
        IdLote = id, NombreMedicamento = "Paracetamol", NumeroLote = $"PAR-{id:000}",
        FechaVencimiento = vence, EstadoLote = EstadoLote.Disponible, StockActual = 10
    };
    private static AlertaRetiro Retiro(int id, int? lote, DateOnly vence) => new()
    {
        IdRetiro = id, IdLote = lote, NombreProducto = "Paracetamol", NumeroLote = "PAR-003",
        FechaVencimiento = vence
    };
    private sealed class RelojFijo : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }
    private sealed class DevolucionesFalso : IDevoluciones
    {
        public bool Pendiente { get; set; }
        public IReadOnlyList<OrdenPendienteDto> OrdenesPendientesDeRemesa(int idLote)
            => Pendiente ? new[] { new OrdenPendienteDto { IdDevolucion = 1 } } : Array.Empty<OrdenPendienteDto>();
    }
    private sealed class LotesFalso : ILotes
    {
        public List<DatosRemesaDto> Items { get; } = new();
        public List<int> Bloqueados { get; } = new();
        public List<int> Liberados { get; } = new();
        public bool FallarAlBloquear { get; set; }
        public IReadOnlyList<DatosRemesaDto> RemesasDisponibles() => Items;
        public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> ids)
            => Items.Where(r => ids.Contains(r.IdLote)).ToArray();
        public void Bloquear(int idLote)
        {
            if (FallarAlBloquear) throw new InvalidOperationException("Fallo simulado en LOT");
            Bloqueados.Add(idLote);
        }
        public void Liberar(int idLote) => Liberados.Add(idLote);
        public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite) => Items;
        public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor) => Items;
        public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles() => throw new NotSupportedException();
        public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto) => throw new NotSupportedException();
        public void Descontar(int idLote, int cantidad) => throw new NotSupportedException();
        public void Restituir(int idLote, int cantidad) => throw new NotSupportedException();
    }
    private sealed class RepositorioFalso : IAlertasRepositorio
    {
        public List<AlertaRetiro> Retiros { get; } = new();
        private int _siguiente = 10;
        public int ObtenerUmbralMesesActivo() => 2;
        public void ActualizarFechaUltimaVerificacion(DateTime fechaHoraLocal) { }
        public IReadOnlyList<AlertaConsulta> ConsultarRetiros() => Array.Empty<AlertaConsulta>();
        public T EjecutarTransaccion<T>(Func<IAlertasUnidadTrabajo, T> operacion)
        {
            var copia = Retiros.ToArray();
            var idAntes = _siguiente;
            try { return operacion(new UnidadFalsa(this)); }
            catch { Retiros.Clear(); Retiros.AddRange(copia); _siguiente = idAntes; throw; }
        }
        private sealed class UnidadFalsa : IAlertasUnidadTrabajo
        {
            private readonly RepositorioFalso _r;
            public UnidadFalsa(RepositorioFalso r) => _r = r;
            public AlertaRetiro? ObtenerRetiro(int idRetiro) => _r.Retiros.FirstOrDefault(x => x.IdRetiro == idRetiro);
            public int Insertar(NuevaAlertaRetiro r)
            {
                var id = ++_r._siguiente;
                _r.Retiros.Add(new AlertaRetiro
                {
                    IdRetiro = id, IdLote = r.IdLote, NombreProducto = r.NombreProducto,
                    NumeroLote = r.NumeroLote, FechaVencimiento = r.FechaVencimiento,
                    EsLoteDefectuoso = r.EsLoteDefectuoso, AlertaDigemid = r.AlertaDigemid
                });
                return id;
            }
            public bool Actualizar(int idRetiro, bool defecto, string? digemid)
            {
                var r = ObtenerRetiro(idRetiro);
                if (r is null) return false;
                _r.Retiros.Remove(r);
                _r.Retiros.Add(new AlertaRetiro
                {
                    IdRetiro = r.IdRetiro, IdLote = r.IdLote, NombreProducto = r.NombreProducto,
                    NumeroLote = r.NumeroLote, FechaVencimiento = r.FechaVencimiento,
                    EsLoteDefectuoso = defecto, AlertaDigemid = digemid
                });
                return true;
            }
            public bool Eliminar(int idRetiro) => _r.Retiros.RemoveAll(x => x.IdRetiro == idRetiro) == 1;
            public int ContarRetiros(int idLote) => _r.Retiros.Count(x => x.IdLote == idLote);
        }
    }
}
