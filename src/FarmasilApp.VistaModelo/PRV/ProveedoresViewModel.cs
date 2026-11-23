using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.PRV;

/// <summary>
/// VistaModelo de Proveedores:
/// ESP-0037 creación, ESP-0038 consulta,
/// ESP-0039 actualización y ESP-0040 baja.
/// </summary>
public sealed class ProveedoresViewModel : ViewModelBase
{
    private const string Todos = "Todos";

    private readonly ProveedoresServicio _servicio;
    private readonly IMensajeServicio _mensajes;
    private readonly ISesion _sesion;

    private string? _busqueda;
    private string _estadoSeleccionado = Todos;
    private ProveedorConsulta? _proveedorSeleccionado;

    private int? _idProveedorFicha;
    private string _ruc = string.Empty;
    private string _razonSocial = string.Empty;
    private string _telefono = string.Empty;
    private string _estadoFicha = string.Empty;
    private bool _enEdicion;
    private string _mensaje = string.Empty;

    public ProveedoresViewModel(
        ProveedoresServicio servicio,
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
                    && ProveedorSeleccionado is not null);

        ConfirmarActualizacionCommand =
            new AsyncRelayCommand(
                ConfirmarActualizacionAsync,
                () => EsAdministrador
                    && EnEdicion
                    && ProveedorSeleccionado is not null);

        EliminarCommand =
            new AsyncRelayCommand(
                DarDeBajaAsync,
                () => EsAdministrador
                    && ProveedorSeleccionado is not null);

        SeleccionarProveedorCommand =
            new RelayCommand<ProveedorConsulta>(
                SeleccionarProveedor,
                proveedor => proveedor is not null);
    }

    private bool EsAdministrador =>
        _sesion.UsuarioActual.Rol == Rol.Administrador;

    /// <summary>
    /// RNF-0006, condición 4: explica por qué las operaciones
    /// de escritura están deshabilitadas para la Técnica.
    /// </summary>
    public string? MotivoAccionesAdministrador =>
        EsAdministrador
            ? null
            : "Solo disponible para el Administrador.";

    public ObservableCollection<ProveedorConsulta> Proveedores { get; } = new();

    public ObservableCollection<string> Estados { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public AsyncRelayCommand CrearCommand { get; }

    public RelayCommand ActualizarCommand { get; }

    public AsyncRelayCommand ConfirmarActualizacionCommand { get; }

    public AsyncRelayCommand EliminarCommand { get; }

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
        set
        {
            if (!Establecer(ref _proveedorSeleccionado, value))
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

    public int? IdProveedorFicha
    {
        get => _idProveedorFicha;
        private set => Establecer(ref _idProveedorFicha, value);
    }

    public string Ruc
    {
        get => _ruc;
        set => Establecer(ref _ruc, value);
    }

    public string RazonSocial
    {
        get => _razonSocial;
        set => Establecer(ref _razonSocial, value);
    }

    public string Telefono
    {
        get => _telefono;
        set => Establecer(ref _telefono, value);
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

            Mensaje = MensajesProveedores.ErrorConsulta;
        }
    }

    private Task CrearAsync() =>
        EjecutarAsync(
            "Creación",
            MensajesProveedores.ErrorOperacion,
            async () =>
            {
                var ruc = Ruc;
                var razonSocial = RazonSocial;
                var telefono = Telefono;

                var idProveedor = await Task.Run(
                    () => _servicio.Crear(
                        ruc,
                        razonSocial,
                        telefono));

                _mensajes.MostrarExito(
                    MensajesProveedores.Titulo,
                    MensajesProveedores.ProveedorRegistrado);

                await RefrescarAsync();

                IdProveedorFicha = idProveedor;
            });

    private void IniciarEdicion()
    {
        if (ProveedorSeleccionado is null)
        {
            return;
        }

        CargarFicha(ProveedorSeleccionado);
        EnEdicion = true;
    }

    private Task ConfirmarActualizacionAsync() =>
        EjecutarAsync(
            "Actualización",
            MensajesProveedores.ErrorOperacion,
            async () =>
            {
                if (ProveedorSeleccionado is not { } proveedor)
                {
                    return;
                }

                var ruc = Ruc;
                var razonSocial = RazonSocial;
                var telefono = Telefono;

                await Task.Run(
                    () => _servicio.Actualizar(
                        proveedor.IdProveedor,
                        ruc,
                        razonSocial,
                        telefono));

                _mensajes.MostrarExito(
                    MensajesProveedores.Titulo,
                    MensajesProveedores.ProveedorActualizado);

                await RefrescarAsync();
            });

    private Task DarDeBajaAsync() =>
        EjecutarAsync(
            "Baja",
            MensajesProveedores.ErrorOperacion,
            async () =>
            {
                if (ProveedorSeleccionado is not { } proveedor)
                {
                    return;
                }

                // Verificar antes del modal (ESP-0040: órdenes pendientes).
                await Task.Run(
                    () => _servicio.VerificarBaja(
                        proveedor.IdProveedor));

                if (!_mensajes.Confirmar(
                        "PRV",
                        null,
                        MensajesProveedores.ConfirmarBaja))
                {
                    return;
                }

                await Task.Run(
                    () => _servicio.DarDeBaja(
                        proveedor.IdProveedor));

                _mensajes.MostrarExito(
                    MensajesProveedores.Titulo,
                    MensajesProveedores.ProveedorDadoDeBaja);

                await RefrescarAsync();
            });

    private async Task RefrescarAsync()
    {
        await ConsultarAsync();
        LimpiarFicha();
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

    private void CargarFicha(ProveedorConsulta? proveedor)
    {
        if (proveedor is null)
        {
            LimpiarFicha();
            return;
        }

        IdProveedorFicha = proveedor.IdProveedor;
        Ruc = proveedor.Ruc;
        RazonSocial = proveedor.RazonSocial;
        Telefono = proveedor.Telefono ?? string.Empty;
        EstadoFicha = proveedor.Estado;
    }

    private void LimpiarFicha()
    {
        IdProveedorFicha = null;
        Ruc = string.Empty;
        RazonSocial = string.Empty;
        Telefono = string.Empty;
        EstadoFicha = string.Empty;
        EnEdicion = false;
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
                MensajesProveedores.Titulo,
                ex.Message);
        }
        catch (AccesoDenegadoException ex)
        {
            RegistrarError(operacion, ex);

            _mensajes.MostrarError(
                MensajesProveedores.Titulo,
                "No tiene permisos para realizar esta operación.");
        }
        catch (Exception ex)
        {
            RegistrarError(operacion, ex);

            _mensajes.MostrarError(
                MensajesProveedores.Titulo,
                mensajeError);
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
