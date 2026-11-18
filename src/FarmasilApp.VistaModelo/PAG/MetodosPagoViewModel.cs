using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.PAG;

/// <summary>
/// VistaModelo de la consulta de métodos de pago (ESP-0018).
/// </summary>
public sealed class MetodosPagoViewModel : ViewModelBase
{
    /// <summary>Opción del filtro de estado que no filtra, como en los demás módulos.</summary>
    public const string TodosLosEstados = "Todos";

    private readonly MetodosPagoServicio _servicio;

    private string? _busqueda;
    private string _estadoFiltro = TodosLosEstados;
    private MetodoPagoConsulta? _metodoSeleccionado;
    private string _mensaje = string.Empty;

    public MetodosPagoViewModel(MetodosPagoServicio servicio)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        FiltrosEstado.Add(TodosLosEstados);
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
            Mensaje = Metodos.Count == 0
                ? "No se encontraron métodos de pago."
                : $"Métodos de pago encontrados: {Metodos.Count}.";
        }
        catch (Exception ex)
        {
            // Cualquier falla se registra y se informa sin el detalle técnico: el comando asíncrono
            // no debe dejar escapar una excepción, porque cerraría la aplicación.
            RegistrarError(ex);
            MostrarErrorConsulta();
        }
    }

    private string? ObtenerEstadoFiltro()
    {
        return string.IsNullOrWhiteSpace(EstadoFiltro)
            || EstadoFiltro == TodosLosEstados
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
