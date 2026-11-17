using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.PRV;

/// <summary>
/// VistaModelo de la consulta de proveedores (ESP-0038).
/// </summary>
public sealed class ProveedoresViewModel : ViewModelBase
{
    private const string Todos = "Todos";

    private readonly ProveedoresServicio _servicio;

    private string? _busqueda;
    private string _estadoSeleccionado = Todos;
    private ProveedorConsulta? _proveedorSeleccionado;
    private string _mensaje = string.Empty;

    public ProveedoresViewModel(ProveedoresServicio servicio)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        InicializarCommand = new AsyncRelayCommand(InicializarAsync);
        BuscarCommand = new AsyncRelayCommand(ConsultarAsync);

        SeleccionarProveedorCommand =
            new RelayCommand<ProveedorConsulta>(
                SeleccionarProveedor,
                proveedor => proveedor is not null);
    }

    public ObservableCollection<ProveedorConsulta> Proveedores { get; } = new();

    public ObservableCollection<string> Estados { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public RelayCommand<ProveedorConsulta> SeleccionarProveedorCommand { get; }

    public string? Busqueda
    {
        get => _busqueda;
        set => Establecer(ref _busqueda, value);
    }

    public string EstadoSeleccionado
    {
        get => _estadoSeleccionado;
        set => Establecer(ref _estadoSeleccionado, value);
    }

    public ProveedorConsulta? ProveedorSeleccionado
    {
        get => _proveedorSeleccionado;
        set => Establecer(ref _proveedorSeleccionado, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    private async Task InicializarAsync()
    {
        CargarFiltros();
        await ConsultarAsync();
    }

    private void CargarFiltros()
    {
        Estados.Clear();
        Estados.Add(Todos);
        Estados.Add("Activo");
        Estados.Add("Inactivo");

        EstadoSeleccionado = Todos;
    }

    private async Task ConsultarAsync()
    {
        try
        {
            var estado = ObtenerEstadoSeleccionado();

            var resultados = await Task.Run(
                () => _servicio.Consultar(
                    Busqueda,
                    estado));

            Proveedores.Clear();

            foreach (var proveedor in resultados)
            {
                Proveedores.Add(proveedor);
            }

            ProveedorSeleccionado = null;

            Mensaje = Proveedores.Count == 0
                ? "No se encontraron proveedores."
                : $"Proveedores encontrados: {Proveedores.Count}.";
        }
        catch (Exception ex)
        {
            RegistrarError("Consulta de proveedores", ex);

            Proveedores.Clear();
            ProveedorSeleccionado = null;

            Mensaje = "No se pudo consultar los proveedores. Inténtelo nuevamente.";
        }
    }

    private string? ObtenerEstadoSeleccionado()
    {
        return string.IsNullOrWhiteSpace(EstadoSeleccionado)
            || EstadoSeleccionado == Todos
                ? null
                : EstadoSeleccionado;
    }

    private void SeleccionarProveedor(ProveedorConsulta? proveedor)
    {
        if (proveedor is not null)
        {
            ProveedorSeleccionado = proveedor;
        }
    }

    private static void RegistrarError(
        string operacion,
        Exception excepcion)
    {
        Trace.TraceError(
            "[Proveedores] {0}: {1}",
            operacion,
            excepcion);
    }
}
