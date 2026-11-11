using System.Collections.ObjectModel;
using FarmasilApp.Modelo.Entidades.INV;
using FarmasilApp.Modelo.Servicios.INV;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.INV;

/// <summary>
/// VistaModelo de la consulta de productos de Inventario (ESP-0006).
/// </summary>
public sealed class InventarioViewModel : ViewModelBase
{
    private readonly InventarioServicio _servicio;

    private string? _busqueda;
    private int? _proveedorSeleccionado;
    private string? _accionTerapeuticaSeleccionada;
    private string _mensaje = string.Empty;

    public InventarioViewModel(InventarioServicio servicio)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        BuscarCommand = new RelayCommand(Consultar);

        CargarFiltros();
        Consultar();
    }

    public ObservableCollection<ProductoInventario> Productos { get; } = new();

    public ObservableCollection<int> Proveedores { get; } = new();

    public ObservableCollection<string> AccionesTerapeuticas { get; } = new();

    public RelayCommand BuscarCommand { get; }

    public string? Busqueda
    {
        get => _busqueda;
        set => Establecer(ref _busqueda, value);
    }

    public int? ProveedorSeleccionado
    {
        get => _proveedorSeleccionado;
        set => Establecer(ref _proveedorSeleccionado, value);
    }

    public string? AccionTerapeuticaSeleccionada
    {
        get => _accionTerapeuticaSeleccionada;
        set => Establecer(ref _accionTerapeuticaSeleccionada, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    private void CargarFiltros()
    {
        try
        {
            Proveedores.Clear();

            foreach (var proveedor in _servicio.ObtenerProveedores())
            {
                Proveedores.Add(proveedor);
            }

            AccionesTerapeuticas.Clear();

            foreach (var accion in _servicio.ObtenerAccionesTerapeuticas())
            {
                AccionesTerapeuticas.Add(accion);
            }
        }
        catch (Exception ex)
        {
            Mensaje = $"No se pudieron cargar los filtros: {ex.Message}";
        }
    }

    private void Consultar()
    {
        try
        {
            var resultados = _servicio.Consultar(
                Busqueda,
                ProveedorSeleccionado,
                AccionTerapeuticaSeleccionada);

            Productos.Clear();

            foreach (var producto in resultados)
            {
                Productos.Add(producto);
            }

            Mensaje = Productos.Count == 0
                ? "No se encontraron productos."
                : $"Productos encontrados: {Productos.Count}.";
        }
        catch (Exception ex)
        {
            Productos.Clear();
            Mensaje = $"No se pudo realizar la consulta: {ex.Message}";
        }
    }
}
