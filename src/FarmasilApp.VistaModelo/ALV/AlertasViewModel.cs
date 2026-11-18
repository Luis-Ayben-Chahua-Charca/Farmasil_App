using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.ALV;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.ALV;

/// <summary>VistaModelo de consulta (ESP-0013) adaptada a los IDs del issue #86.</summary>
public sealed class AlertasViewModel : ViewModelBase
{
    private readonly AlertasServicio _servicio;
    private AlertaFiltroLote? _loteSeleccionado;
    private string _estadoSeleccionado = "Todos";
    private string _mensaje = string.Empty;
    private int _umbralMeses;
    private int _totalProximasAVencer;
    private DateOnly _fechaLimite;
    private bool _filtrosCargados;

    public AlertasViewModel(AlertasServicio servicio)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
        Estados.Add("Todos");
        Estados.Add("Por vencer");
        Estados.Add("Retirados");
        var todos = AlertaFiltroLote.Todos;
        Lotes.Add(todos);
        LoteSeleccionado = todos;
        InicializarCommand = new AsyncRelayCommand(ConsultarAsync);
        ConsultarCommand = new AsyncRelayCommand(ConsultarAsync);
    }

    public ObservableCollection<AlertaConsulta> Alertas { get; } = new();
    public ObservableCollection<AlertaConsulta> Tarjetas { get; } = new();
    public ObservableCollection<AlertaFiltroLote> Lotes { get; } = new();
    public ObservableCollection<string> Estados { get; } = new();
    public AsyncRelayCommand InicializarCommand { get; }
    public AsyncRelayCommand ConsultarCommand { get; }

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

    private async Task ConsultarAsync()
    {
        try
        {
            var idLote = LoteSeleccionado?.IdLote;
            var estado = EstadoSeleccionado;
            var resultado = await Task.Run(() => _servicio.Consultar(idLote, estado));
            Alertas.Clear();
            Tarjetas.Clear();
            foreach (var alerta in resultado.Alertas)
            {
                Alertas.Add(alerta);
                Tarjetas.Add(alerta);
            }
            UmbralMeses = resultado.UmbralMeses;
            FechaLimite = resultado.FechaLimite;
            TotalProximasAVencer = resultado.TotalProximasAVencer;
            Mensaje = Alertas.Count == 0
                ? "No se encontraron alertas con los filtros seleccionados."
                : $"Resultados: {Alertas.Count}. Próximas a vencer (sin filtros): {TotalProximasAVencer}.";

            if (!_filtrosCargados)
            {
                // La primera consulta sin filtros carga las opciones de lote del tablero.
                foreach (var lote in resultado.Alertas.Where(a => a.IdLote.HasValue)
                    .GroupBy(a => a.IdLote!.Value)
                    .OrderBy(g => g.First().NumeroLote))
                {
                    var a = lote.First();
                    Lotes.Add(new AlertaFiltroLote
                    {
                        IdLote = lote.Key,
                        Descripcion = $"{a.NumeroLote} — {a.Medicamento}"
                    });
                }
                _filtrosCargados = true;
            }
        }
        catch (Exception ex)
        {
            Trace.TraceError("[ALV] Consulta de alertas: {0}", ex);
            Alertas.Clear();
            Tarjetas.Clear();
            Mensaje = "No se pudieron consultar las alertas. Compruebe la conexión y la regla activa.";
        }
    }
}
