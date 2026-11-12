using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.INV;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.INV;

/// <summary>
/// VistaModelo de la consulta de productos de Inventario (ESP-0006).
/// </summary>
public sealed class InventarioViewModel : ViewModelBase
{
    private const string Todos = "Todos";

    private readonly InventarioServicio _servicio;

    private string? _busqueda;
    private string _proveedorSeleccionado = Todos;
    private string _accionTerapeuticaSeleccionada = Todos;
    private ProductoInventario? _productoSeleccionado;
    private string _mensaje = string.Empty;

    public InventarioViewModel(InventarioServicio servicio)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        InicializarCommand = new AsyncRelayCommand(InicializarAsync);
        BuscarCommand = new AsyncRelayCommand(ConsultarAsync);

        SeleccionarProductoCommand =
            new RelayCommand<ProductoInventario>(
                SeleccionarProducto,
                producto => producto is not null);
    }

    public ObservableCollection<ProductoInventario> Productos { get; } = new();

    public ObservableCollection<string> Proveedores { get; } = new();

    public ObservableCollection<string> AccionesTerapeuticas { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public RelayCommand<ProductoInventario> SeleccionarProductoCommand { get; }

    public string? Busqueda
    {
        get => _busqueda;
        set => Establecer(ref _busqueda, value);
    }

    public string ProveedorSeleccionado
    {
        get => _proveedorSeleccionado;
        set => Establecer(ref _proveedorSeleccionado, value);
    }

    public string AccionTerapeuticaSeleccionada
    {
        get => _accionTerapeuticaSeleccionada;
        set => Establecer(ref _accionTerapeuticaSeleccionada, value);
    }

    public ProductoInventario? ProductoSeleccionado
    {
        get => _productoSeleccionado;
        set => Establecer(ref _productoSeleccionado, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    private async Task InicializarAsync()
    {
        var filtrosCargados = await CargarFiltrosAsync();
        await ConsultarAsync();

        // La consulta reemplaza el mensaje: el aviso de los filtros se antepone para que no se pierda.
        if (!filtrosCargados)
        {
            Mensaje = $"No se pudieron cargar los filtros del inventario. {Mensaje}";
        }
    }

    private async Task<bool> CargarFiltrosAsync()
    {
        try
        {
            var proveedores = await Task.Run(
                () => _servicio.ObtenerProveedores());

            var acciones = await Task.Run(
                () => _servicio.ObtenerAccionesTerapeuticas());

            Proveedores.Clear();
            Proveedores.Add(Todos);

            foreach (var proveedor in proveedores)
            {
                Proveedores.Add(proveedor.ToString());
            }

            AccionesTerapeuticas.Clear();
            AccionesTerapeuticas.Add(Todos);

            foreach (var accion in acciones)
            {
                AccionesTerapeuticas.Add(accion);
            }

            ProveedorSeleccionado = Todos;
            AccionTerapeuticaSeleccionada = Todos;

            return true;
        }
        catch (Exception ex)
        {
            RegistrarError("Carga de filtros", ex);

            Proveedores.Clear();
            Proveedores.Add(Todos);

            AccionesTerapeuticas.Clear();
            AccionesTerapeuticas.Add(Todos);

            return false;
        }
    }

    private async Task ConsultarAsync()
    {
        try
        {
            var proveedor = ObtenerProveedorSeleccionado();
            var accion = ObtenerAccionTerapeuticaSeleccionada();

            var resultados = await Task.Run(
                () => _servicio.Consultar(
                    Busqueda,
                    proveedor,
                    accion));

            Productos.Clear();

            foreach (var producto in resultados)
            {
                Productos.Add(producto);
            }

            ProductoSeleccionado = null;

            Mensaje = Productos.Count == 0
                ? "No se encontraron productos."
                : $"Productos encontrados: {Productos.Count}.";
        }
        catch (Exception ex)
        {
            RegistrarError("Consulta de productos", ex);

            Productos.Clear();
            ProductoSeleccionado = null;

            Mensaje = "No se pudo consultar el inventario. Inténtelo nuevamente.";
        }
    }

    private int? ObtenerProveedorSeleccionado()
    {
        if (string.IsNullOrWhiteSpace(ProveedorSeleccionado)
            || ProveedorSeleccionado == Todos)
        {
            return null;
        }

        return int.TryParse(
            ProveedorSeleccionado,
            out var idProveedor)
            ? idProveedor
            : null;
    }

    private string? ObtenerAccionTerapeuticaSeleccionada()
    {
        return string.IsNullOrWhiteSpace(AccionTerapeuticaSeleccionada)
            || AccionTerapeuticaSeleccionada == Todos
                ? null
                : AccionTerapeuticaSeleccionada;
    }

    private void SeleccionarProducto(ProductoInventario? producto)
    {
        if (producto is not null)
        {
            ProductoSeleccionado = producto;
        }
    }

    private static void RegistrarError(
        string operacion,
        Exception excepcion)
    {
        Trace.TraceError(
            "[Inventario] {0}: {1}",
            operacion,
            excepcion);
    }
}
