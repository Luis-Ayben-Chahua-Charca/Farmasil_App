using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.PAG;

/// <summary>
/// VistaModelo de la consulta de métodos de pago (ESP-0018).
/// </summary>
public sealed class MetodosPagoViewModel : ViewModelBase
{
    private const string SinFiltroEstado = "-- Filtrar por Estado --";

    private readonly MetodosPagoServicio _servicio;

    private string? _busqueda;
    private string _estadoFiltro = SinFiltroEstado;
    private MetodoPagoConsulta? _metodoSeleccionado;
    private string _mensaje = string.Empty;

    public MetodosPagoViewModel(MetodosPagoServicio servicio)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        FiltrosEstado.Add(SinFiltroEstado);
        FiltrosEstado.Add("Activo");
        FiltrosEstado.Add("Inactivo");

        EstadosMetodo.Add("Activo");
        EstadosMetodo.Add("Inactivo");

        InicializarCommand = new AsyncRelayCommand(InicializarAsync);
        BuscarCommand = new AsyncRelayCommand(ConsultarAsync);
    }

    public ObservableCollection<MetodoPagoConsulta> Metodos { get; } = new();

    public ObservableCollection<string> FiltrosEstado { get; } = new();

    public ObservableCollection<string> EstadosMetodo { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public string? Busqueda
    {
        get => _busqueda;
        set => Establecer(ref _busqueda, value);
    }

    public string EstadoFiltro
    {
        get => _estadoFiltro;
        set => Establecer(ref _estadoFiltro, value);
    }

    public MetodoPagoConsulta? MetodoSeleccionado
    {
        get => _metodoSeleccionado;
        set => Establecer(ref _metodoSeleccionado, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    private async Task InicializarAsync()
    {
        await ConsultarAsync();
    }

    private async Task ConsultarAsync()
    {
        try
        {
            var estado = ObtenerEstadoFiltro();
            var resultados = await Task.Run(
                () => _servicio.Consultar(Busqueda, estado));

            Metodos.Clear();

            foreach (var metodo in resultados)
            {
                Metodos.Add(metodo);
            }

            MetodoSeleccionado = null;
            Mensaje = string.Empty;
        }
        catch (DbException ex)
        {
            RegistrarError(ex);
            MostrarErrorConsulta();
        }
        catch (InvalidOperationException ex)
        {
            RegistrarError(ex);
            MostrarErrorConsulta();
        }
    }

    private string? ObtenerEstadoFiltro()
    {
        return string.IsNullOrWhiteSpace(EstadoFiltro)
            || EstadoFiltro == SinFiltroEstado
                ? null
                : EstadoFiltro;
    }

    private void MostrarErrorConsulta()
    {
        Metodos.Clear();
        MetodoSeleccionado = null;
        Mensaje = "No se pudieron consultar los métodos de pago. Inténtelo nuevamente.";
    }

    private static void RegistrarError(Exception excepcion)
    {
        Trace.TraceError("[Métodos de pago] Consulta: {0}", excepcion);
    }
}
