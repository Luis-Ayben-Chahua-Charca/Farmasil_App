using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.ALV;

/// <summary>VistaModelo ALV: ESP-0013/0014/0015/0016. Toda escritura pasa por AlertasServicio.</summary>
public sealed class AlertasViewModel : ViewModelBase
{
    private readonly AlertasServicio _servicio;
    private readonly IMensajeServicio _mensajes;
    private readonly ISesion _sesion;
    private IReadOnlyList<DatosRemesaDto> _remesas = Array.Empty<DatosRemesaDto>();
    private AlertaFiltroLote? _loteSeleccionado;
    private AlertaFiltroLote? _loteFichaSeleccionado;
    private AlertaConsulta? _alertaSeleccionada;
    private string _estadoSeleccionado = "Todos";
    private string _codigoDigemid = string.Empty;
    private string _mensaje = string.Empty;
    private string _fechaFicha = string.Empty;
    private bool _defectuoso;
    private bool _enEdicion;
    private int _umbralMeses;
    private int _totalProximasAVencer;
    private DateOnly _fechaLimite;
    private AlertaFiltroLote? _loteDelRetiro;

    public AlertasViewModel(AlertasServicio servicio, IMensajeServicio mensajes, ISesion sesion)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
        _mensajes = mensajes ?? throw new ArgumentNullException(nameof(mensajes));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
        Estados.Add("Todos"); Estados.Add("Por vencer"); Estados.Add("Retirados");
        LoteSeleccionado = AlertaFiltroLote.Todos;
        Lotes.Add(AlertaFiltroLote.Todos);
        InicializarCommand = new AsyncRelayCommand(ConsultarAsync);
        ConsultarCommand = new AsyncRelayCommand(ConsultarAsync);
        CrearCommand = new AsyncRelayCommand(CrearAsync, () => PuedeCrear);
        ActualizarCommand = new RelayCommand(IniciarEdicion, () => PuedeModificar);
        ConfirmarActualizacionCommand = new AsyncRelayCommand(GuardarAsync, () => EnEdicion && PuedeModificar);
        EliminarCommand = new AsyncRelayCommand(EliminarAsync, () => PuedeEliminar);
        SeleccionarAlertaCommand = new RelayCommand<AlertaConsulta>(a => AlertaSeleccionada = a);
    }

    public ObservableCollection<AlertaConsulta> Alertas { get; } = new();
    public ObservableCollection<AlertaConsulta> Tarjetas { get; } = new();
    public ObservableCollection<AlertaFiltroLote> Lotes { get; } = new();
    public ObservableCollection<AlertaFiltroLote> LotesFicha { get; } = new();
    public ObservableCollection<string> Estados { get; } = new();
    public AsyncRelayCommand InicializarCommand { get; }
    public AsyncRelayCommand ConsultarCommand { get; }
    public AsyncRelayCommand CrearCommand { get; }
    public RelayCommand ActualizarCommand { get; }
    public AsyncRelayCommand ConfirmarActualizacionCommand { get; }
    public AsyncRelayCommand EliminarCommand { get; }
    public RelayCommand<AlertaConsulta> SeleccionarAlertaCommand { get; }

    public bool EsAdministrador => _sesion.UsuarioActual.Rol == Rol.Administrador;
    public bool PuedeCrear => _sesion.UsuarioActual.Rol is Rol.Administrador or Rol.Tecnico;
    public bool PuedeModificar => EsAdministrador && AlertaSeleccionada?.IdRetiro is > 0;
    public bool PuedeEliminar => PuedeModificar;
    public bool PuedeElegirRemesa => !EnEdicion;
    public string MotivoSinPermiso => MensajesAlertas.SoloAdministrador;

    public AlertaFiltroLote? LoteSeleccionado
    {
        get => _loteSeleccionado;
        set => Establecer(ref _loteSeleccionado, value);
    }
    public string EstadoSeleccionado
    {
        get => _estadoSeleccionado;
        set => Establecer(ref _estadoSeleccionado, value);
    }
    public AlertaFiltroLote? LoteFichaSeleccionado
    {
        get => _loteFichaSeleccionado;
        set
        {
            if (Establecer(ref _loteFichaSeleccionado, value) && !EnEdicion)
            {
                var remesa = _remesas.FirstOrDefault(r => r.IdLote == value?.IdLote);
                FechaFicha = remesa is null ? string.Empty : remesa.FechaVencimiento.ToString("dd/MM/yyyy");
            }
        }
    }
    public AlertaConsulta? AlertaSeleccionada
    {
        get => _alertaSeleccionada;
        set
        {
            if (!Establecer(ref _alertaSeleccionada, value)) return;
            QuitarLoteDelRetiro();
            EnEdicion = false;
            Defectuoso = value?.EsLoteDefectuoso ?? false;
            CodigoDigemid = value?.AlertaDigemid ?? string.Empty;
            if (value != null)
            {
                LoteFichaSeleccionado = BuscarOAgregarLoteDelRetiro(value);
                FechaFicha = value.FechaVencimiento.ToString("dd/MM/yyyy");
            }
            ActualizarBotones();
        }
    }
    public string FechaFicha
    {
        get => _fechaFicha;
        private set => Establecer(ref _fechaFicha, value);
    }
    public bool Defectuoso
    {
        get => _defectuoso;
        set => Establecer(ref _defectuoso, value);
    }
    public string CodigoDigemid
    {
        get => _codigoDigemid;
        set => Establecer(ref _codigoDigemid, value ?? string.Empty);
    }
    public bool EnEdicion
    {
        get => _enEdicion;
        private set
        {
            if (Establecer(ref _enEdicion, value))
            {
                Notificar(nameof(PuedeElegirRemesa));
                ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
            }
        }
    }
    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }
    public int UmbralMeses
    {
        get => _umbralMeses;
        private set => Establecer(ref _umbralMeses, value);
    }
    public DateOnly FechaLimite
    {
        get => _fechaLimite;
        private set => Establecer(ref _fechaLimite, value);
    }
    public int TotalProximasAVencer
    {
        get => _totalProximasAVencer;
        private set => Establecer(ref _totalProximasAVencer, value);
    }

    private Task ConsultarAsync() => EjecutarAsync("Consulta", async () =>
    {
        var filtro = LoteSeleccionado?.IdLote;
        var estado = EstadoSeleccionado;
        var resultado = await Task.Run(() => _servicio.Consultar(filtro, estado));
        MostrarResultado(resultado);
        // Remesas disponibles para nueva alerta: contrato de LOT, independiente del filtro de consulta.
        var remesas = await Task.Run(_servicio.RemesasParaRegistrar);
        _remesas = remesas;
        LotesFicha.Clear();
        foreach (var r in remesas)
            LotesFicha.Add(new AlertaFiltroLote { IdLote = r.IdLote, Descripcion = $"{r.NumeroLote} — {r.NombreMedicamento}" });
        Notificar(nameof(EsAdministrador));
        ActualizarBotones();
    });

    private void MostrarResultado(ResultadoAlertas resultado)
    {
        Alertas.Clear(); Tarjetas.Clear();
        foreach (var alerta in resultado.Alertas) { Alertas.Add(alerta); Tarjetas.Add(alerta); }
        UmbralMeses = resultado.UmbralMeses;
        FechaLimite = resultado.FechaLimite;
        TotalProximasAVencer = resultado.TotalProximasAVencer;
        Lotes.Clear();
        Lotes.Add(AlertaFiltroLote.Todos);
        foreach (var grupo in resultado.Alertas.Where(a => a.IdLote.HasValue)
            .GroupBy(a => a.IdLote!.Value).OrderBy(a => a.First().NumeroLote))
        {
            var muestra = grupo.First();
            Lotes.Add(new AlertaFiltroLote { IdLote = grupo.Key, Descripcion = $"{muestra.NumeroLote} — {muestra.Medicamento}" });
        }
        if (LoteSeleccionado?.IdLote is int id)
            LoteSeleccionado = Lotes.FirstOrDefault(x => x.IdLote == id) ?? Lotes[0];
        else LoteSeleccionado = Lotes[0];
        AlertaSeleccionada = null;
        LoteFichaSeleccionado = null;
        FechaFicha = string.Empty;
        EnEdicion = false;
        Mensaje = Alertas.Count == 0
            ? "No se encontraron alertas con los filtros seleccionados."
            : $"Resultados: {Alertas.Count}. Próximas a vencer (sin filtros): {TotalProximasAVencer}.";
    }

    private Task CrearAsync() => EjecutarAsync("Registro", async () =>
    {
        var idLote = LoteFichaSeleccionado?.IdLote ?? 0;
        var defecto = Defectuoso;
        var codigo = CodigoDigemid;
        await Task.Run(() => _servicio.Registrar(idLote, defecto, codigo));
        _mensajes.MostrarExito(MensajesAlertas.Titulo, MensajesAlertas.Registrada);
        await ConsultarAsync();
    });

    private void IniciarEdicion()
    {
        if (!PuedeModificar) return;
        EnEdicion = true; // la remesa queda inmutable, solo se editan defecto y DIGEMID.
    }

    private Task GuardarAsync() => EjecutarAsync("Actualización", async () =>
    {
        if (AlertaSeleccionada?.IdRetiro is not int id) return;
        await Task.Run(() => _servicio.Modificar(id, Defectuoso, CodigoDigemid));
        _mensajes.MostrarExito(MensajesAlertas.Titulo, MensajesAlertas.Actualizada);
        await ConsultarAsync();
    });

    private Task EliminarAsync() => EjecutarAsync("Eliminación", async () =>
    {
        if (AlertaSeleccionada?.IdRetiro is not int id) return;
        // Mismo patrón de Usuarios: comprobar antes de abrir el modal, repetir dentro de la transacción.
        await Task.Run(() => _servicio.VerificarEliminacion(id));
        if (!_mensajes.Confirmar("ALV", null, MensajesAlertas.ConfirmarEliminacion)) return;
        var mensaje = await Task.Run(() => _servicio.Eliminar(id));
        _mensajes.MostrarExito(MensajesAlertas.Titulo, mensaje);
        await ConsultarAsync();
    });

    /// <summary>
    /// Una remesa con alerta ya no está entre las disponibles que ofrece LOT (queda bloqueada), pero ESP-0015 pide
    /// mostrarla en solo lectura: se agrega al combo mientras su retiro está seleccionado.
    /// </summary>
    private AlertaFiltroLote? BuscarOAgregarLoteDelRetiro(AlertaConsulta alerta)
    {
        if (alerta.IdLote is not int idLote) return null;
        var existente = LotesFicha.FirstOrDefault(l => l.IdLote == idLote);
        if (existente is not null) return existente;
        _loteDelRetiro = new AlertaFiltroLote
        {
            IdLote = idLote,
            Descripcion = $"{alerta.NumeroLote} — {alerta.Medicamento}"
        };
        LotesFicha.Add(_loteDelRetiro);
        return _loteDelRetiro;
    }

    private void QuitarLoteDelRetiro()
    {
        if (_loteDelRetiro is null) return;
        var lote = _loteDelRetiro;
        _loteDelRetiro = null;
        LotesFicha.Remove(lote);
    }

    private void ActualizarBotones()
    {
        Notificar(nameof(PuedeCrear));
        Notificar(nameof(PuedeModificar));
        Notificar(nameof(PuedeEliminar));
        CrearCommand.NotificarPuedeEjecutar();
        ActualizarCommand.NotificarPuedeEjecutar();
        ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
        EliminarCommand.NotificarPuedeEjecutar();
    }

    private async Task EjecutarAsync(string operacion, Func<Task> accion)
    {
        try { await accion(); }
        catch (ReglaDeNegocioException ex) { _mensajes.MostrarError(MensajesAlertas.Titulo, ex.Message); }
        catch (AccesoDenegadoException ex)
        {
            Trace.TraceError("[ALV] {0}: {1}", operacion, ex);
            _mensajes.MostrarError(MensajesAlertas.Titulo, MensajesAlertas.SoloAdministrador);
        }
        catch (Exception ex)
        {
            Trace.TraceError("[ALV] {0}: {1}", operacion, ex);
            _mensajes.MostrarError(MensajesAlertas.Titulo, MensajesAlertas.ErrorTecnico);
        }
    }
}
