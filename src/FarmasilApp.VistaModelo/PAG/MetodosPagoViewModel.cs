using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.PAG;

/// <summary>
/// VistaModelo de la pantalla única de Métodos de Pago (ESP-0017 a ESP-0020).
/// </summary>
public sealed class MetodosPagoViewModel : ViewModelBase
{
    public const string TodosLosEstados = "Todos";
    private const string EstadoPorDefecto = "Activo";

    private readonly MetodosPagoServicio _servicio;
    private readonly IMensajeServicio _mensajes;
    private readonly ISesion _sesion;

    private string? _busqueda;
    private string _estadoFiltro = TodosLosEstados;
    private MetodoPagoConsulta? _metodoSeleccionado;
    private string _nombreMetodo = string.Empty;
    private string _estadoFicha = EstadoPorDefecto;
    private bool _enEdicion;
    private string _mensaje = string.Empty;

    public MetodosPagoViewModel(
        MetodosPagoServicio servicio,
        IMensajeServicio mensajes,
        ISesion sesion)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
        _mensajes = mensajes ?? throw new ArgumentNullException(nameof(mensajes));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));

        InicializarCommand = new AsyncRelayCommand(InicializarAsync);
        BuscarCommand = new AsyncRelayCommand(ConsultarAsync);
        CrearCommand = new AsyncRelayCommand(CrearAsync, () => EsAdministrador && !EnEdicion);
        ActualizarCommand = new RelayCommand(
            IniciarEdicion,
            () => EsAdministrador && MetodoSeleccionado is not null && !EnEdicion);
        ConfirmarActualizacionCommand = new AsyncRelayCommand(
            ConfirmarActualizacionAsync,
            () => EsAdministrador && EnEdicion && MetodoSeleccionado is not null);
        EliminarCommand = new AsyncRelayCommand(
            DarDeBajaAsync,
            () => EsAdministrador && MetodoSeleccionado is not null && !EnEdicion);
    }

    public ObservableCollection<MetodoPagoConsulta> Metodos { get; } = new();

    public IReadOnlyList<string> FiltrosEstado { get; } =
        new[] { TodosLosEstados, "Activo", "Inactivo" };

    public IReadOnlyList<string> EstadosFicha { get; } = new[] { "Activo", "Inactivo" };

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public AsyncRelayCommand CrearCommand { get; }

    public RelayCommand ActualizarCommand { get; }

    public AsyncRelayCommand ConfirmarActualizacionCommand { get; }

    public AsyncRelayCommand EliminarCommand { get; }

    public bool EsAdministrador => _sesion.UsuarioActual.Rol == Rol.Administrador;

    public string? MotivoAccionesAdministrador =>
        EsAdministrador ? null : "Solo disponible para el Administrador.";

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
        set
        {
            if (!Establecer(ref _metodoSeleccionado, value))
            {
                return;
            }

            EnEdicion = false;
            CargarFicha(value);
            NotificarComandos();
        }
    }

    public string NombreMetodo
    {
        get => _nombreMetodo;
        set => Establecer(ref _nombreMetodo, value);
    }

    public string EstadoFicha
    {
        get => _estadoFicha;
        set => Establecer(ref _estadoFicha, value);
    }

    public bool EnEdicion
    {
        get => _enEdicion;
        private set
        {
            if (Establecer(ref _enEdicion, value))
            {
                NotificarComandos();
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
        await ConsultarAsync();
    }

    private async Task ConsultarAsync()
    {
        try
        {
            var estado = ObtenerEstadoFiltro();
            var resultados = await Task.Run(
                () => _servicio.Consultar(Busqueda, estado));

            MostrarMetodos(resultados);
            Mensaje = Metodos.Count == 0
                ? "No se encontraron métodos de pago."
                : $"Métodos de pago encontrados: {Metodos.Count}.";
        }
        catch (ReglaDeNegocioException ex)
        {
            RegistrarError("Consulta", ex);
            MostrarMetodos(Array.Empty<MetodoPagoConsulta>());
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            RegistrarError("Consulta", ex);
            MostrarMetodos(Array.Empty<MetodoPagoConsulta>());
            Mensaje = MensajesMetodosPago.ErrorConsulta;
        }
    }

    private Task CrearAsync() => EjecutarAsync("Creación", async () =>
    {
        var nombre = NombreMetodo;
        var estado = EstadoFicha;

        await Task.Run(() => _servicio.Crear(nombre, estado));

        _mensajes.MostrarExito(MensajesMetodosPago.Titulo, MensajesMetodosPago.MetodoRegistrado);
        await RefrescarAsync();
    });

    private void IniciarEdicion()
    {
        if (MetodoSeleccionado is null)
        {
            return;
        }

        CargarFicha(MetodoSeleccionado);
        EnEdicion = true;
    }

    private Task ConfirmarActualizacionAsync() => EjecutarAsync("Actualización", async () =>
    {
        if (MetodoSeleccionado is not { } metodo)
        {
            return;
        }

        var nombre = NombreMetodo;
        var estado = EstadoFicha;
        await Task.Run(() => _servicio.Actualizar(metodo.IdMetodoPago, nombre, estado));

        _mensajes.MostrarExito(MensajesMetodosPago.Titulo, MensajesMetodosPago.MetodoActualizado);
        await RefrescarAsync();
    });

    private Task DarDeBajaAsync() => EjecutarAsync("Baja", async () =>
    {
        if (MetodoSeleccionado is not { } metodo)
        {
            return;
        }

        if (!_mensajes.Confirmar("PAG", null, MensajesMetodosPago.ConfirmarBaja))
        {
            return;
        }

        await Task.Run(() => _servicio.DarDeBaja(metodo.IdMetodoPago));

        _mensajes.MostrarExito(MensajesMetodosPago.Titulo, MensajesMetodosPago.MetodoDadoDeBaja);
        await RefrescarAsync();
    });

    private async Task RefrescarAsync()
    {
        await ConsultarAsync();
    }

    private void MostrarMetodos(IReadOnlyList<MetodoPagoConsulta> metodos)
    {
        Metodos.Clear();
        foreach (var metodo in metodos)
        {
            Metodos.Add(metodo);
        }

        MetodoSeleccionado = null;
        CargarFicha(null);
    }

    private void CargarFicha(MetodoPagoConsulta? metodo)
    {
        NombreMetodo = metodo?.NombreMetodo ?? string.Empty;
        EstadoFicha = metodo?.Estado ?? EstadoPorDefecto;
    }

    private string? ObtenerEstadoFiltro()
    {
        return string.IsNullOrWhiteSpace(EstadoFiltro) || EstadoFiltro == TodosLosEstados
            ? null
            : EstadoFiltro;
    }

    private async Task EjecutarAsync(string operacion, Func<Task> accion)
    {
        try
        {
            await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            _mensajes.MostrarError(MensajesMetodosPago.Titulo, ex.Message);
        }
        catch (AccesoDenegadoException ex)
        {
            RegistrarError(operacion, ex);
            _mensajes.MostrarError(MensajesMetodosPago.Titulo, "No tiene permisos para realizar esta operación.");
        }
        catch (Exception ex)
        {
            RegistrarError(operacion, ex);
            _mensajes.MostrarError(MensajesMetodosPago.Titulo, MensajesMetodosPago.ErrorOperacion);
        }
    }

    private static void RegistrarError(string operacion, Exception excepcion)
    {
        Trace.TraceError("[Métodos de pago] {0}: {1}", operacion, excepcion);
    }

    private void NotificarComandos()
    {
        CrearCommand.NotificarPuedeEjecutar();
        ActualizarCommand.NotificarPuedeEjecutar();
        ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
        EliminarCommand.NotificarPuedeEjecutar();
    }
}
