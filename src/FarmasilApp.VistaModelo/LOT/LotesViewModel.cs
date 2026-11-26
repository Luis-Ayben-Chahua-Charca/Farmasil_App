using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.LOT;

/// <summary>
/// VistaModelo del módulo de Lotes:
/// ESP-0041 alta, ESP-0042 consulta,
/// ESP-0043 actualización y ESP-0044 baja.
/// </summary>
public sealed class LotesViewModel : ViewModelBase
{
    private const string Todos = "Todos";

    private readonly LotesServicio _servicio;
    private readonly IMensajeServicio _mensajes;
    private readonly ISesion _sesion;

    private MedicamentoDto? _medicamentoFiltro;
    private string? _numeroLoteFiltro;
    private string _estadoFiltro = Todos;
    private RemesaConsulta? _remesaSeleccionada;

    private int? _idLoteFicha;
    private MedicamentoDto? _medicamentoFicha;
    private ProveedorDto? _proveedorFicha;
    private string _numeroLote = string.Empty;
    private DateTime? _fechaVencimiento;
    private string _stock = string.Empty;
    private DateTime? _fechaIngreso;
    private string _estadoFicha = "Disponible";
    private bool _enEdicion;
    private string _mensaje = string.Empty;

    public LotesViewModel(
        LotesServicio servicio,
        IMensajeServicio mensajes,
        ISesion sesion)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        _mensajes = mensajes
            ?? throw new ArgumentNullException(nameof(mensajes));

        _sesion = sesion
            ?? throw new ArgumentNullException(nameof(sesion));

        InicializarCommand =
            new AsyncRelayCommand(InicializarAsync);

        BuscarCommand =
            new AsyncRelayCommand(ConsultarAsync);

        CrearCommand =
            new AsyncRelayCommand(
                CrearAsync,
                () => EsAdministrador);

        ActualizarCommand =
            new RelayCommand(
                IniciarEdicion,
                () => EsAdministrador
                    && RemesaSeleccionada is not null);

        ConfirmarActualizacionCommand =
            new AsyncRelayCommand(
                ConfirmarActualizacionAsync,
                () => EsAdministrador
                    && EnEdicion
                    && RemesaSeleccionada is not null);

        EliminarCommand =
            new AsyncRelayCommand(
                DarDeBajaAsync,
                () => EsAdministrador
                    && RemesaSeleccionada is not null);
    }

    private bool EsAdministrador =>
        _sesion.UsuarioActual.Rol == Rol.Administrador;

    public string? MotivoAccionesAdministrador =>
        EsAdministrador
            ? null
            : "Solo disponible para el Administrador.";

    public ObservableCollection<RemesaConsulta> Remesas { get; } = new();

    public ObservableCollection<MedicamentoDto> Medicamentos { get; } = new();

    public ObservableCollection<ProveedorDto> Proveedores { get; } = new();

    public ObservableCollection<string> EstadosFiltro { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }


    public AsyncRelayCommand CrearCommand { get; }

    public RelayCommand ActualizarCommand { get; }

    public AsyncRelayCommand ConfirmarActualizacionCommand { get; }

    public AsyncRelayCommand EliminarCommand { get; }


    public MedicamentoDto? MedicamentoFiltro
    {
        get => _medicamentoFiltro;
        set => Establecer(ref _medicamentoFiltro, value);
    }

    public string? NumeroLoteFiltro
    {
        get => _numeroLoteFiltro;
        set => Establecer(ref _numeroLoteFiltro, value);
    }

    public string EstadoFiltro
    {
        get => _estadoFiltro;
        set => Establecer(ref _estadoFiltro, value);
    }

    public RemesaConsulta? RemesaSeleccionada
    {
        get => _remesaSeleccionada;
        set
        {
            if (!Establecer(ref _remesaSeleccionada, value))
            {
                return;
            }

            EnEdicion = false;
            CargarFicha(value);

            ActualizarCommand.NotificarPuedeEjecutar();
            ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
            EliminarCommand.NotificarPuedeEjecutar();
            CrearCommand.NotificarPuedeEjecutar();
        }
    }

    public int? IdLoteFicha
    {
        get => _idLoteFicha;
        private set
        {
            if (Establecer(ref _idLoteFicha, value))
            {
                CrearCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public MedicamentoDto? MedicamentoFicha
    {
        get => _medicamentoFicha;
        set => Establecer(ref _medicamentoFicha, value);
    }

    public ProveedorDto? ProveedorFicha
    {
        get => _proveedorFicha;
        set => Establecer(ref _proveedorFicha, value);
    }

    public string NumeroLote
    {
        get => _numeroLote;
        set => Establecer(ref _numeroLote, value);
    }

    public DateTime? FechaVencimiento
    {
        get => _fechaVencimiento;
        set => Establecer(ref _fechaVencimiento, value);
    }

    public string Stock
    {
        get => _stock;
        set => Establecer(ref _stock, value);
    }

    public DateTime? FechaIngreso
    {
        get => _fechaIngreso;
        set => Establecer(ref _fechaIngreso, value);
    }

    public string EstadoFicha
    {
        get => _estadoFicha;
        private set => Establecer(ref _estadoFicha, value);
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

    public bool EsNueva =>
        IdLoteFicha is null;

    private async Task InicializarAsync()
    {
        await CargarCatalogosAsync();
        CargarEstados();
        PrepararNueva();
        await ConsultarAsync();
    }

    private async Task CargarCatalogosAsync()
    {
        var medicamentos = await Task.Run(
            () => _servicio.MedicamentosDisponibles());

        var proveedores = await Task.Run(
            () => _servicio.ProveedoresActivos());

        Medicamentos.Clear();

        foreach (var medicamento in medicamentos)
        {
            Medicamentos.Add(medicamento);
        }

        Proveedores.Clear();

        foreach (var proveedor in proveedores)
        {
            Proveedores.Add(proveedor);
        }
    }

    private void CargarEstados()
    {
        EstadosFiltro.Clear();
        EstadosFiltro.Add(Todos);
        EstadosFiltro.Add("Disponible");
        EstadosFiltro.Add("Bloqueado por devolucion");
        EstadosFiltro.Add("Agotado");

        EstadoFiltro = Todos;
    }

    private async Task ConsultarAsync()
    {
        try
        {
            var estado =
                ObtenerEstadoFiltro();

            var resultados = await Task.Run(
                () => _servicio.Consultar(
                    MedicamentoFiltro?.IdProducto,
                    NumeroLoteFiltro,
                    estado));

            Remesas.Clear();

            foreach (var remesa in resultados)
            {
                Remesas.Add(remesa);
            }

            RemesaSeleccionada = null;

            Mensaje = Remesas.Count == 0
                ? "No se encontraron remesas."
                : $"Remesas encontradas: {Remesas.Count}.";
        }
        catch (Exception ex)
        {
            RegistrarError(
                "Consulta de remesas",
                ex);

            Remesas.Clear();
            RemesaSeleccionada = null;

            Mensaje =
                "No fue posible consultar las remesas.";
        }
    }

    private Task CrearAsync() =>
        EjecutarAsync(
            "Registro de remesa",
            async () =>
            {
                var stock =
                    ObtenerStockParaCrear();

                var vencimiento =
                    ObtenerFechaVencimiento();

                var ingreso =
                    ObtenerFechaIngreso();

                await Task.Run(
                    () => _servicio.Crear(
                        MedicamentoFicha?.IdProducto,
                        ProveedorFicha?.IdProveedor,
                        NumeroLote,
                        vencimiento,
                        stock,
                        ingreso));

                _mensajes.MostrarExito(
                    "Lotes",
                    MensajesLotes.Registrada);

                await RefrescarAsync();
            });

    private void IniciarEdicion()
    {
        if (RemesaSeleccionada is null)
        {
            return;
        }

        CargarFicha(RemesaSeleccionada);
        EnEdicion = true;
    }

    private Task ConfirmarActualizacionAsync() =>
        EjecutarAsync(
            "Actualización de remesa",
            async () =>
            {
                if (RemesaSeleccionada is not { } remesa)
                {
                    return;
                }

                var stock =
                    ObtenerStockParaActualizar();

                var vencimiento =
                    ObtenerFechaVencimiento();

                await Task.Run(
                    () => _servicio.Actualizar(
                        remesa.IdLote,
                        ProveedorFicha?.IdProveedor,
                        NumeroLote,
                        vencimiento,
                        stock));

                _mensajes.MostrarExito(
                    "Lotes",
                    MensajesLotes.Actualizada);

                await RefrescarAsync();
            });

    private Task DarDeBajaAsync() =>
        EjecutarAsync(
            "Baja de remesa",
            async () =>
            {
                if (RemesaSeleccionada is not { } remesa)
                {
                    return;
                }

                await Task.Run(
                    () => _servicio.VerificarBaja(
                        remesa.IdLote));

                if (!_mensajes.Confirmar(
                        "LOT",
                        null,
                        MensajesLotes.ConfirmarBaja))
                {
                    return;
                }

                await Task.Run(
                    () => _servicio.DarDeBaja(
                        remesa.IdLote));

                _mensajes.MostrarExito(
                    "Lotes",
                    MensajesLotes.DadaDeBaja);

                await RefrescarAsync();
            });

    private void CargarFicha(
        RemesaConsulta? remesa)
    {
        if (remesa is null)
        {
            return;
        }

        IdLoteFicha =
            remesa.IdLote;

        MedicamentoFicha =
            Medicamentos.FirstOrDefault(
                x => x.IdProducto
                    == remesa.IdProducto);

        ProveedorFicha =
            Proveedores.FirstOrDefault(
                x => x.IdProveedor
                    == remesa.IdProveedor);

        NumeroLote =
            remesa.NumeroLote;

        FechaVencimiento =
            remesa.FechaVencimiento
                .ToDateTime(
                    TimeOnly.MinValue);

        Stock =
            remesa.StockActual.ToString();

        FechaIngreso =
            remesa.FechaIngreso
                .ToDateTime(
                    TimeOnly.MinValue);

        EstadoFicha =
            TextoEstado(
                remesa.Estado);
    }

    private void PrepararNueva()
    {
        RemesaSeleccionada = null;
        IdLoteFicha = null;

        MedicamentoFicha =
            Medicamentos.FirstOrDefault();

        if (MedicamentoFicha is not null)
        {
            ProveedorFicha =
                Proveedores.FirstOrDefault(
                    x => x.IdProveedor
                        == MedicamentoFicha.IdProveedor)
                ?? Proveedores.FirstOrDefault();
        }
        else
        {
            ProveedorFicha =
                Proveedores.FirstOrDefault();
        }

        NumeroLote = string.Empty;
        FechaVencimiento = null;
        Stock = string.Empty;
        FechaIngreso = DateTime.Today;
        EstadoFicha = "Disponible";
        EnEdicion = false;

        ActualizarCommand.NotificarPuedeEjecutar();
        ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
        EliminarCommand.NotificarPuedeEjecutar();
        CrearCommand.NotificarPuedeEjecutar();
    }

    private async Task RefrescarAsync()
    {
        await CargarCatalogosAsync();
        await ConsultarAsync();
        PrepararNueva();
    }

    private EstadoLote? ObtenerEstadoFiltro()
    {
        return EstadoFiltro switch
        {
            Todos => null,

            "Disponible" =>
                EstadoLote.Disponible,

            "Bloqueado por devolucion" =>
                EstadoLote.BloqueadoPorDevolucion,

            "Agotado" =>
                EstadoLote.Agotado,

            _ =>
                null,
        };
    }

    private int ObtenerStockParaCrear()
    {
        if (!int.TryParse(
                Stock,
                out var stock)
            || stock <= 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.StockInicialInvalido);
        }

        return stock;
    }

    private int ObtenerStockParaActualizar()
    {
        if (!int.TryParse(
                Stock,
                out var stock)
            || stock < 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.StockInvalido);
        }

        return stock;
    }

    private DateOnly ObtenerFechaVencimiento()
    {
        if (!FechaVencimiento.HasValue)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.FechaVencimientoInvalida);
        }

        return DateOnly.FromDateTime(
            FechaVencimiento.Value);
    }

    private DateOnly ObtenerFechaIngreso()
    {
        if (!FechaIngreso.HasValue)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.FechaIngresoInvalida);
        }

        return DateOnly.FromDateTime(
            FechaIngreso.Value);
    }

    private async Task EjecutarAsync(
        string operacion,
        Func<Task> accion)
    {
        try
        {
            await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            _mensajes.MostrarError(
                "Lotes",
                ex.Message);
        }
        catch (AccesoDenegadoException ex)
        {
            RegistrarError(
                operacion,
                ex);

            _mensajes.MostrarError(
                "Lotes",
                "No tiene permisos para realizar esta operación.");
        }
        catch (Exception ex)
        {
            RegistrarError(
                operacion,
                ex);

            _mensajes.MostrarError(
                "Lotes",
                "No fue posible completar la operación.");
        }
    }

    private static string TextoEstado(
        EstadoLote estado)
    {
        return estado switch
        {
            EstadoLote.Disponible =>
                "Disponible",

            EstadoLote.BloqueadoPorDevolucion =>
                "Bloqueado por devolucion",

            EstadoLote.Agotado =>
                "Agotado",

            _ =>
                estado.ToString(),
        };
    }

    private static void RegistrarError(
        string operacion,
        Exception excepcion)
    {
        Trace.TraceError(
            "[Lotes] {0}: {1}",
            operacion,
            excepcion);
    }
}
