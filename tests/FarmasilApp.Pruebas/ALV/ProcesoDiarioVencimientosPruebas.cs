using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.ALV;

public sealed class ProcesoDiarioVencimientosPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 26);

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public void ElAvisoCuentaElDiaLimiteYElAnteriorPeroNoElPosterior()
    {
        var repositorio = new RepositorioFalso { Umbral = 2 };
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, Hoy.AddMonths(2).AddDays(-1)));
        lotes.Items.Add(Remesa(2, Hoy.AddMonths(2)));
        lotes.Items.Add(Remesa(3, Hoy.AddMonths(2).AddDays(1)));
        lotes.Items.Add(Remesa(4, Hoy.AddDays(-2))); // también se cuenta el vencido disponible con stock
        lotes.Items.Add(Remesa(5, Hoy.AddDays(1), EstadoLote.BloqueadoPorDevolucion));
        lotes.Items.Add(Remesa(6, Hoy.AddDays(1), EstadoLote.Agotado, 0));
        lotes.Items.Add(Remesa(7, Hoy.AddDays(1), EstadoLote.Disponible, 0));
        var reloj = new RelojFijo();
        var canal = new CanalAvisosVencimientos();
        AvisoVencimientos? recibido = null;
        canal.AvisoPublicado += (_, aviso) => recibido = aviso;

        var resultado = new EvaluacionVencimientosServicio(repositorio, lotes, reloj, canal).Evaluar();

        Assert.Equal(3, resultado.CantidadRemesas);
        Assert.Same(resultado, recibido);
        Assert.Equal(Hoy.AddMonths(2), resultado.FechaLimite);
        Assert.Equal(reloj.GetLocalNow().DateTime, repositorio.UltimaVerificacion);
        Assert.Equal(1, repositorio.TotalActualizaciones);
        Assert.Equal(0, repositorio.TotalConsultasRetiros);
        Assert.Equal(0, repositorio.TotalEscriturasRetiros);
        Assert.Equal(1, lotes.TotalConsultasPorVencer);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public void ElConteoDiarioCoincideConLaConsultaEsp0013SinFiltros()
    {
        var repositorio = new RepositorioFalso { Umbral = 2 };
        var lotes = new LotesFalsos();
        lotes.Items.Add(Remesa(1, Hoy.AddMonths(1)));
        lotes.Items.Add(Remesa(2, Hoy.AddMonths(2)));
        lotes.Items.Add(Remesa(3, Hoy.AddDays(1), EstadoLote.BloqueadoPorDevolucion));
        var reloj = new RelojFijo();
        var servicioConsulta = new AlertasServicio(repositorio, lotes,
            new SesionSimulada(Rol.Tecnico), reloj, new DevolucionesSimulado(reloj));
        var conteo = new EvaluacionVencimientosServicio(repositorio, lotes, reloj,
            new CanalAvisosVencimientos()).Evaluar().CantidadRemesas;

        Assert.Equal(servicioConsulta.Consultar().TotalProximasAVencer, conteo);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public void SiFallaLaConsultaNoActualizaFechaNiPublicaAviso()
    {
        var repositorio = new RepositorioFalso { FallarAlLeer = true };
        var canal = new CanalAvisosVencimientos();
        var eventos = 0;
        canal.AvisoPublicado += (_, _) => eventos++;
        var servicio = new EvaluacionVencimientosServicio(repositorio,
            new LotesFalsos(), new RelojFijo(), canal);

        Assert.Throws<InvalidOperationException>(() => servicio.Evaluar());
        Assert.Null(repositorio.UltimaVerificacion);
        Assert.Equal(0, repositorio.TotalActualizaciones);
        Assert.Equal(0, eventos);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public void SiFallaLaActualizacionNoSePublicaElAviso()
    {
        var repositorio = new RepositorioFalso { FallarAlActualizar = true };
        var canal = new CanalAvisosVencimientos();
        var eventos = 0;
        canal.AvisoPublicado += (_, _) => eventos++;
        var servicio = new EvaluacionVencimientosServicio(repositorio,
            new LotesFalsos(), new RelojFijo(), canal);

        Assert.Throws<InvalidOperationException>(() => servicio.Evaluar());
        Assert.Null(repositorio.UltimaVerificacion);
        Assert.Equal(0, eventos);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public async Task EjecutaAlIniciarYLuegoCadaVeinticuatroHorasConEsperaControlada()
    {
        var repositorio = new RepositorioFalso();
        var canal = new CanalAvisosVencimientos();
        var primerAviso = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var segundoAviso = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var primeraEspera = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var segundaEspera = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cantidadEsperas = 0;
        var primeraEsperaIniciada = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var duraciones = new List<TimeSpan>();
        var total = 0;
        canal.AvisoPublicado += (_, _) =>
        {
            if (Interlocked.Increment(ref total) == 1) primerAviso.TrySetResult(true);
            else segundoAviso.TrySetResult(true);
        };
        var evaluador = new EvaluacionVencimientosServicio(repositorio,
            new LotesFalsos(), new RelojFijo(), canal);
        var proceso = new ProcesoDiarioVencimientos(evaluador, (duracion, token) =>
        {
            lock (duraciones) duraciones.Add(duracion);
            primeraEsperaIniciada.TrySetResult(true);
            return Interlocked.Increment(ref cantidadEsperas) == 1
                ? primeraEspera.Task.WaitAsync(token)
                : segundaEspera.Task.WaitAsync(token);
        });

        using var cancelacion = new CancellationTokenSource();
        var tarea = proceso.EjecutarAsync(cancelacion.Token);
        await primerAviso.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await primeraEsperaIniciada.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, Volatile.Read(ref total));
        primeraEspera.TrySetResult(true); // avanzar las 24 horas sin esperar tiempo real
        await segundoAviso.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancelacion.Cancel();
        await tarea.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(2, Volatile.Read(ref total));
        Assert.Equal(2, repositorio.TotalActualizaciones);
        Assert.All(duraciones, duracion => Assert.Equal(TimeSpan.FromHours(24), duracion));
    }

    [Fact]
    [Trait("Especificacion", "RNF-0010")]
    [Trait("Tecnica", "T-001")]
    public async Task UnFalloInicialPermiteReintentarEnElSiguienteCiclo()
    {
        var repositorio = new RepositorioFalso { FallosPendientesAlLeer = 1 };
        var canal = new CanalAvisosVencimientos();
        var publicado = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        canal.AvisoPublicado += (_, _) => publicado.TrySetResult(true);
        var ciclo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var otroCiclo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var numeroEspera = 0;
        var esperaIniciada = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var evaluador = new EvaluacionVencimientosServicio(repositorio,
            new LotesFalsos(), new RelojFijo(), canal);
        var proceso = new ProcesoDiarioVencimientos(evaluador, (duracion, token) =>
        {
            esperaIniciada.TrySetResult(true);
            return Interlocked.Increment(ref numeroEspera) == 1
                ? ciclo.Task.WaitAsync(token)
                : otroCiclo.Task.WaitAsync(token);
        });

        using var cancelacion = new CancellationTokenSource();
        var tarea = proceso.EjecutarAsync(cancelacion.Token);
        await esperaIniciada.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Null(repositorio.UltimaVerificacion);
        ciclo.TrySetResult(true);
        await publicado.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancelacion.Cancel();
        await tarea.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, repositorio.TotalActualizaciones);
    }

    private static DatosRemesaDto Remesa(int id, DateOnly vence,
        EstadoLote estado = EstadoLote.Disponible, int stock = 10) => new()
    {
        IdLote = id, NumeroLote = $"LOTE-{id}", NombreMedicamento = "Medicamento de prueba",
        FechaVencimiento = vence, EstadoLote = estado, StockActual = stock
    };

    private sealed class RelojFijo : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new(2026, 11, 26, 14, 30, 0, TimeSpan.Zero);
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private sealed class RepositorioFalso : IAlertasRepositorio
    {
        public int Umbral { get; set; } = 2;
        public bool FallarAlLeer { get; set; }
        public bool FallarAlActualizar { get; set; }
        public int FallosPendientesAlLeer { get; set; }
        public DateTime? UltimaVerificacion { get; private set; }
        public int TotalActualizaciones { get; private set; }
        public int TotalConsultasRetiros { get; private set; }
        public int TotalEscriturasRetiros { get; private set; }
        public int ObtenerUmbralMesesActivo()
        {
            if (FallarAlLeer || FallosPendientesAlLeer-- > 0)
                throw new InvalidOperationException("Fallo controlado al leer la regla.");
            return Umbral;
        }
        public void ActualizarFechaUltimaVerificacion(DateTime fechaHoraLocal)
        {
            if (FallarAlActualizar) throw new InvalidOperationException("Fallo controlado al actualizar.");
            UltimaVerificacion = fechaHoraLocal;
            TotalActualizaciones++;
        }
        public IReadOnlyList<AlertaConsulta> ConsultarRetiros()
        {
            TotalConsultasRetiros++;
            return Array.Empty<AlertaConsulta>();
        }
        public T EjecutarTransaccion<T>(Func<IAlertasUnidadTrabajo, T> operacion)
        {
            TotalEscriturasRetiros++;
            throw new NotSupportedException("El proceso diario no puede escribir retiros.");
        }
    }

    private sealed class LotesFalsos : ILotes
    {
        public List<DatosRemesaDto> Items { get; } = new();
        public int TotalConsultasPorVencer { get; private set; }
        public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite)
        {
            TotalConsultasPorVencer++;
            return Items.ToArray(); // la evaluación también protege el predicado de la ESP
        }
        public IReadOnlyList<DatosRemesaDto> RemesasDisponibles() => throw new NotSupportedException();
        public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote) => throw new NotSupportedException();
        public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor) => throw new NotSupportedException();
        public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles() => throw new NotSupportedException();
        public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto) => throw new NotSupportedException();
        public void Bloquear(int idLote) => throw new NotSupportedException();
        public void Liberar(int idLote) => throw new NotSupportedException();
        public void Descontar(int idLote, int cantidad) => throw new NotSupportedException();
        public void Restituir(int idLote, int cantidad) => throw new NotSupportedException();
    }
}
