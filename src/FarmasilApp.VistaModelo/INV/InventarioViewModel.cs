using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.INV;

/// <summary>
/// VistaModelo de Inventario:
/// ESP-0005 creaciÃƒÂ³n, ESP-0006 consulta,
/// ESP-0007 actualizaciÃƒÂ³n y ESP-0008 baja.
/// </summary>
public sealed class InventarioViewModel : ViewModelBase
{
    private const string Todos = "Todos";

    private readonly InventarioServicio _servicio;
    private readonly IMensajeServicio _mensajes;

    private string? _busqueda;
    private ProveedorFiltro? _proveedorSeleccionado = ProveedorFiltro.Todos;
    private string _accionTerapeuticaSeleccionada = Todos;
    private ProductoInventario? _productoSeleccionado;

    private int? _idProductoFicha;
    private string _nombreProducto = string.Empty;
    private string _accionTerapeuticaFicha = string.Empty;
    private string _precioVentaTexto = string.Empty;
    private ProveedorDto? _proveedorFicha;
    private string _stockMinimoTexto = string.Empty;
    private bool _enEdicion;
    private string _mensaje = string.Empty;

    public InventarioViewModel(
        InventarioServicio servicio,
        IMensajeServicio mensajes)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        _mensajes = mensajes
            ?? throw new ArgumentNullException(nameof(mensajes));

        InicializarCommand =
            new AsyncRelayCommand(InicializarAsync);

        BuscarCommand =
            new AsyncRelayCommand(ConsultarAsync);

        CrearCommand =
            new AsyncRelayCommand(CrearAsync);

        ActualizarCommand =
            new RelayCommand(
                IniciarEdicion,
                () => ProductoSeleccionado is not null);

        ConfirmarActualizacionCommand =
            new AsyncRelayCommand(
                ConfirmarActualizacionAsync,
                () => EnEdicion && ProductoSeleccionado is not null);

        EliminarCommand =
            new AsyncRelayCommand(
                DarDeBajaAsync,
                () => ProductoSeleccionado is not null);

        SeleccionarProductoCommand =
            new RelayCommand<ProductoInventario>(
                SeleccionarProducto,
                producto => producto is not null);
    }

    public ObservableCollection<ProductoInventario> Productos { get; } = new();

    public ObservableCollection<ProveedorFiltro> Proveedores { get; } = new();

    public ObservableCollection<string> AccionesTerapeuticas { get; } = new();

    public ObservableCollection<ProveedorDto> ProveedoresActivos { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public AsyncRelayCommand CrearCommand { get; }

    public RelayCommand ActualizarCommand { get; }

    public AsyncRelayCommand ConfirmarActualizacionCommand { get; }

    public AsyncRelayCommand EliminarCommand { get; }

    public RelayCommand<ProductoInventario> SeleccionarProductoCommand { get; }

    public string? Busqueda
    {
        get => _busqueda;
        set => Establecer(ref _busqueda, value);
    }

    public ProveedorFiltro? ProveedorSeleccionado
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
        set
        {
            if (!Establecer(ref _productoSeleccionado, value))
            {
                return;
            }

            EnEdicion = false;
            CargarFicha(value);

            ActualizarCommand.NotificarPuedeEjecutar();
            ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
            EliminarCommand.NotificarPuedeEjecutar();
        }
    }

    public int? IdProductoFicha
    {
        get => _idProductoFicha;
        private set => Establecer(ref _idProductoFicha, value);
    }

    public string NombreProducto
    {
        get => _nombreProducto;
        set => Establecer(ref _nombreProducto, value);
    }

    public string AccionTerapeuticaFicha
    {
        get => _accionTerapeuticaFicha;
        set => Establecer(ref _accionTerapeuticaFicha, value);
    }

    public string PrecioVentaTexto
    {
        get => _precioVentaTexto;
        set => Establecer(ref _precioVentaTexto, value);
    }

    public ProveedorDto? ProveedorFicha
    {
        get => _proveedorFicha;
        set => Establecer(ref _proveedorFicha, value);
    }

    public string StockMinimoTexto
    {
        get => _stockMinimoTexto;
        set => Establecer(ref _stockMinimoTexto, value);
    }

    public bool EnEdicion
    {
        get => _enEdicion;
        private set
        {
            if (Establecer(ref _enEdicion, value))
            {
                ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    private async Task InicializarAsync()
    {
        var filtrosCargados = await CargarFiltrosYProveedoresAsync();

        await ConsultarAsync();

        if (!filtrosCargados)
        {
            Mensaje =
                $"No se pudieron cargar los filtros del inventario. {Mensaje}";
        }
    }

    private async Task<bool> CargarFiltrosYProveedoresAsync()
    {
        try
        {
            var proveedoresFiltro = await Task.Run(
                () => _servicio.ObtenerProveedores());

            var acciones = await Task.Run(
                () => _servicio.ObtenerAccionesTerapeuticas());

            var proveedoresActivos = await Task.Run(
                () => _servicio.ObtenerProveedoresActivos());

            Proveedores.Clear();
            Proveedores.Add(ProveedorFiltro.Todos);

            foreach (var proveedor in proveedoresFiltro)
            {
                Proveedores.Add(proveedor);
            }

            AccionesTerapeuticas.Clear();
            AccionesTerapeuticas.Add(Todos);

            foreach (var accion in acciones)
            {
                AccionesTerapeuticas.Add(accion);
            }

            ProveedoresActivos.Clear();

            foreach (var proveedor in proveedoresActivos)
            {
                ProveedoresActivos.Add(proveedor);
            }

            ProveedorSeleccionado = ProveedorFiltro.Todos;
            AccionTerapeuticaSeleccionada = Todos;

            return true;
        }
        catch (Exception ex)
        {
            RegistrarError("Carga de filtros", ex);

            Proveedores.Clear();
            Proveedores.Add(ProveedorFiltro.Todos);

            AccionesTerapeuticas.Clear();
            AccionesTerapeuticas.Add(Todos);

            ProveedoresActivos.Clear();

            return false;
        }
    }

    private async Task ConsultarAsync()
    {
        try
        {
            var proveedor =
                ProveedorSeleccionado?.IdProveedor;

            var accion =
                string.IsNullOrWhiteSpace(AccionTerapeuticaSeleccionada)
                || AccionTerapeuticaSeleccionada == Todos
                    ? null
                    : AccionTerapeuticaSeleccionada;

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

            Mensaje =
                "No se pudo consultar el inventario. IntÃƒÂ©ntelo nuevamente.";
        }
    }

    private Task CrearAsync() =>
        EjecutarAsync(
            "CreaciÃƒÂ³n",
            "No se pudo registrar el producto.",
            async () =>
            {
                var precio = ObtenerPrecio();
                var stockMinimo = ObtenerStockMinimo();

                var nombre = NombreProducto;
                var accion = AccionTerapeuticaFicha;
                var proveedor = ProveedorFicha?.IdProveedor;

                var idProducto = await Task.Run(
                    () => _servicio.Crear(
                        nombre,
                        accion,
                        precio,
                        proveedor,
                        stockMinimo));

                _mensajes.MostrarExito(
                    "Inventario",
                    "Producto registrado correctamente");

                await RefrescarAsync();

                // ESP-0005: el identificador generado debe mostrarse
                // después de registrar y refrescar el catálogo.
                IdProductoFicha = idProducto;
            });

    private void IniciarEdicion()
    {
        if (ProductoSeleccionado is null)
        {
            return;
        }

        CargarFicha(ProductoSeleccionado);
        EnEdicion = true;
    }

    private Task ConfirmarActualizacionAsync() =>
        EjecutarAsync(
            "ActualizaciÃƒÂ³n",
            "No se pudo actualizar el producto.",
            async () =>
            {
                if (ProductoSeleccionado is not { } producto)
                {
                    return;
                }

                var precio = ObtenerPrecio();
                var stockMinimo = ObtenerStockMinimo();

                var nombre = NombreProducto;
                var accion = AccionTerapeuticaFicha;
                var proveedor = ProveedorFicha?.IdProveedor;

                await Task.Run(
                    () => _servicio.Actualizar(
                        producto.IdProducto,
                        nombre,
                        accion,
                        precio,
                        proveedor,
                        stockMinimo));

                _mensajes.MostrarExito(
                    "Inventario",
                    "Producto actualizado correctamente");

                await RefrescarAsync();
            });

    private Task DarDeBajaAsync() =>
        EjecutarAsync(
            "Baja",
            "No se pudo dar de baja el producto.",
            async () =>
            {
                if (ProductoSeleccionado is not { } producto)
                {
                    return;
                }

                if (!_mensajes.Confirmar(
                        "INV",
                        null,
                        "\u00BFEst\u00E1 seguro de dar de baja este producto?"))
                {
                    return;
                }

                await Task.Run(
                    () => _servicio.DarDeBaja(
                        producto.IdProducto));

                _mensajes.MostrarExito(
                    "Inventario",
                    "Producto dado de baja correctamente");

                await RefrescarAsync();
            });

    private async Task RefrescarAsync()
    {
        await CargarFiltrosYProveedoresAsync();
        await ConsultarAsync();
        LimpiarFicha();
    }

    private void SeleccionarProducto(
        ProductoInventario? producto)
    {
        if (producto is not null)
        {
            ProductoSeleccionado = producto;
        }
    }

    private void CargarFicha(
        ProductoInventario? producto)
    {
        if (producto is null)
        {
            LimpiarFicha();
            return;
        }

        IdProductoFicha = producto.IdProducto;
        NombreProducto = producto.Nombre;
        AccionTerapeuticaFicha = producto.AccionTerapeutica;
        PrecioVentaTexto =
            producto.PrecioVenta.ToString(
                "0.00",
                CultureInfo.CurrentCulture);

        StockMinimoTexto =
            producto.StockMinimo.ToString(
                CultureInfo.CurrentCulture);

        ProveedorFicha =
            ProveedoresActivos.FirstOrDefault(
                p => p.IdProveedor == producto.IdProveedor);
    }

    private void LimpiarFicha()
    {
        IdProductoFicha = null;
        NombreProducto = string.Empty;
        AccionTerapeuticaFicha = string.Empty;
        PrecioVentaTexto = string.Empty;
        StockMinimoTexto = string.Empty;
        ProveedorFicha = null;
        EnEdicion = false;
    }

    private decimal ObtenerPrecio()
    {
        if (!decimal.TryParse(
                PrecioVentaTexto,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out var precio))
        {
            throw new ReglaDeNegocioException(
                "El precio de venta debe ser un nÃƒÂºmero vÃƒÂ¡lido.");
        }

        return precio;
    }

    private int ObtenerStockMinimo()
    {
        if (!int.TryParse(
                StockMinimoTexto,
                NumberStyles.Integer,
                CultureInfo.CurrentCulture,
                out var stockMinimo))
        {
            throw new ReglaDeNegocioException(
                "El stock mÃƒÂ­nimo debe ser un nÃƒÂºmero entero vÃƒÂ¡lido.");
        }

        return stockMinimo;
    }

    private async Task EjecutarAsync(
        string operacion,
        string mensajeError,
        Func<Task> accion)
    {
        try
        {
            await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            _mensajes.MostrarError(
                "Inventario",
                ex.Message);
        }
        catch (AccesoDenegadoException ex)
        {
            RegistrarError(operacion, ex);

            _mensajes.MostrarError(
                "Inventario",
                "No tiene permisos para realizar esta operaciÃƒÂ³n.");
        }
        catch (Exception ex)
        {
            RegistrarError(operacion, ex);

            _mensajes.MostrarError(
                "Inventario",
                mensajeError);
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