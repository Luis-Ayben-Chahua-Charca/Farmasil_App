using System.Collections.ObjectModel;
using System.Diagnostics;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.USR;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.USR;

/// <summary>
/// VistaModelo de la pantalla única de Usuarios, <c>ART-MKP-USR-0001</c> (ESP-0033 a ESP-0036).
/// </summary>
/// <remarks>
/// <para><b>Qué hace y qué no.</b> Enlaza la pantalla con <see cref="UsuariosServicio"/>: toma los datos de la ficha,
/// llama al servicio y muestra el resultado. No contiene reglas de negocio; si una operación debe cancelarse, lo decide
/// el servicio.</para>
/// <para><b>Errores (DEC-072).</b> Toda operación pasa por <see cref="EjecutarAsync"/>:
/// <list type="bullet">
/// <item><see cref="ReglaDeNegocioException"/>: se muestra su mensaje, que es el texto de la especificación.</item>
/// <item><see cref="AccesoDenegadoException"/>: se registra y se muestra <see cref="MensajesUsuarios.AccesoDenegado"/>.</item>
/// <item>Cualquier otra: se registra con <see cref="Trace"/> y se muestra un mensaje genérico, nunca el texto técnico.</item>
/// </list></para>
/// <para><b>Ficha.</b> Seleccionar una tarjeta o una fila carga la ficha (ESP-0034). «Actualizar» la pasa a edición y
/// limpia la contraseña (ESP-0035); «Guardar cambios» solo se habilita en edición. «Crear» registra lo que tenga la
/// ficha (ESP-0033).</para>
/// </remarks>
public sealed class UsuariosViewModel : ViewModelBase
{
    /// <summary>Opción del filtro de rol que no filtra.</summary>
    public const string TodosLosRoles = "Todos";

    private readonly UsuariosServicio _servicio;
    private readonly IMensajeServicio _mensajes;

    private string? _busqueda;
    private string _filtroRolSeleccionado = TodosLosRoles;
    private CuentaUsuario? _cuentaSeleccionada;
    private string _nombreUsuario = string.Empty;
    private string _contrasena = string.Empty;
    private Rol? _rol;
    private EstadoRegistro? _estado = EstadoRegistro.Activo;
    private bool _enEdicion;
    private string _mensaje = string.Empty;

    public UsuariosViewModel(UsuariosServicio servicio, IMensajeServicio mensajes)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
        _mensajes = mensajes ?? throw new ArgumentNullException(nameof(mensajes));

        InicializarCommand = new AsyncRelayCommand(ConsultarAsync);
        BuscarCommand = new AsyncRelayCommand(ConsultarAsync);
        CrearCommand = new AsyncRelayCommand(CrearAsync);
        ActualizarCommand = new RelayCommand(IniciarEdicion, () => CuentaSeleccionada is not null);
        ConfirmarActualizacionCommand = new AsyncRelayCommand(ConfirmarActualizacionAsync, () => EnEdicion && CuentaSeleccionada is not null);
        EliminarCommand = new AsyncRelayCommand(DarDeBajaAsync, () => CuentaSeleccionada is not null);
    }

    /// <summary>Resultado de la consulta: alimenta a la vez USR-PNL-CUENTAS y USR-TBL-USUARIOS.</summary>
    public ObservableCollection<CuentaUsuario> Cuentas { get; } = new();

    /// <summary>Opciones de USR-CMB-FILTRO-ROL.</summary>
    public IReadOnlyList<string> FiltrosRol { get; } = new[] { TodosLosRoles, nameof(Rol.Administrador), nameof(Rol.Tecnico) };

    /// <summary>Opciones de USR-CMB-ROL.</summary>
    public IReadOnlyList<Rol> Roles { get; } = Enum.GetValues<Rol>();

    /// <summary>Opciones de USR-CMB-ESTADO-USUARIO.</summary>
    public IReadOnlyList<EstadoRegistro> Estados { get; } = Enum.GetValues<EstadoRegistro>();

    public AsyncRelayCommand InicializarCommand { get; }

    public AsyncRelayCommand BuscarCommand { get; }

    public AsyncRelayCommand CrearCommand { get; }

    public RelayCommand ActualizarCommand { get; }

    public AsyncRelayCommand ConfirmarActualizacionCommand { get; }

    public AsyncRelayCommand EliminarCommand { get; }

    /// <summary>USR-SRH-USUARIO.</summary>
    public string? Busqueda
    {
        get => _busqueda;
        set => Establecer(ref _busqueda, value);
    }

    /// <summary>USR-CMB-FILTRO-ROL.</summary>
    public string FiltroRolSeleccionado
    {
        get => _filtroRolSeleccionado;
        set => Establecer(ref _filtroRolSeleccionado, value);
    }

    /// <summary>Cuenta elegida en una tarjeta o en una fila. Al cambiar, carga la ficha y sale de la edición.</summary>
    public CuentaUsuario? CuentaSeleccionada
    {
        get => _cuentaSeleccionada;
        set
        {
            if (!Establecer(ref _cuentaSeleccionada, value))
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

    /// <summary>USR-TXT-NOMBRE-USUARIO.</summary>
    public string NombreUsuario
    {
        get => _nombreUsuario;
        set => Establecer(ref _nombreUsuario, value);
    }

    /// <summary>USR-PWD-CONTRASENA. Se enlaza con <c>ContrasenaEnlazada</c>, porque <c>PasswordBox.Password</c> no es enlazable.</summary>
    public string Contrasena
    {
        get => _contrasena;
        set => Establecer(ref _contrasena, value ?? string.Empty);
    }

    /// <summary>USR-CMB-ROL. Nulo mientras no se elija.</summary>
    public Rol? RolFicha
    {
        get => _rol;
        set => Establecer(ref _rol, value);
    }

    /// <summary>USR-CMB-ESTADO-USUARIO. Empieza en 'Activo', el valor por defecto de la tabla.</summary>
    public EstadoRegistro? EstadoFicha
    {
        get => _estado;
        set => Establecer(ref _estado, value);
    }

    /// <summary>La ficha está editando la cuenta seleccionada (después de USR-BTN-ACTUALIZAR-USUARIO).</summary>
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

    /// <summary>Resumen de la última consulta.</summary>
    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    private Task ConsultarAsync() => EjecutarAsync("Consulta", MensajesUsuarios.ErrorConsulta, async () =>
    {
        var nombre = Busqueda;
        var rol = FiltroRolSeleccionado == TodosLosRoles ? (Rol?)null : Enum.Parse<Rol>(FiltroRolSeleccionado);

        var cuentas = await Task.Run(() => _servicio.Consultar(nombre, rol));
        MostrarCuentas(cuentas);
    });

    private Task CrearAsync() => EjecutarAsync("Creación", MensajesUsuarios.ErrorOperacion, async () =>
    {
        var (nombre, contrasena, rol, estado) = (NombreUsuario, Contrasena, RolFicha, EstadoFicha);

        await Task.Run(() => _servicio.Crear(nombre, contrasena, rol, estado));

        _mensajes.MostrarExito(MensajesUsuarios.Titulo, MensajesUsuarios.UsuarioRegistrado);
        await RefrescarAsync();
    });

    private void IniciarEdicion()
    {
        if (CuentaSeleccionada is null)
        {
            return;
        }

        // ESP-0035: carga la cuenta vigente y deja la contraseña en blanco.
        CargarFicha(CuentaSeleccionada);
        EnEdicion = true;
    }

    private Task ConfirmarActualizacionAsync() => EjecutarAsync("Actualización", MensajesUsuarios.ErrorOperacion, async () =>
    {
        if (CuentaSeleccionada is not { } cuenta)
        {
            return;
        }

        var (nombre, contrasena, rol, estado) = (NombreUsuario, Contrasena, RolFicha, EstadoFicha);

        await Task.Run(() => _servicio.Actualizar(cuenta.IdUsuario, nombre, contrasena, rol, estado));

        _mensajes.MostrarExito(MensajesUsuarios.Titulo, MensajesUsuarios.UsuarioActualizado);
        await RefrescarAsync();
    });

    private Task DarDeBajaAsync() => EjecutarAsync("Baja", MensajesUsuarios.ErrorOperacion, async () =>
    {
        if (CuentaSeleccionada is not { } cuenta)
        {
            return;
        }

        // ESP-0036: la cuenta propia se rechaza antes de mostrar el modal.
        await Task.Run(() => _servicio.VerificarBaja(cuenta.IdUsuario));

        if (!_mensajes.Confirmar("USR", null, MensajesUsuarios.ConfirmarBaja))
        {
            return;
        }

        await Task.Run(() => _servicio.DarDeBaja(cuenta.IdUsuario));

        _mensajes.MostrarExito(MensajesUsuarios.Titulo, MensajesUsuarios.CuentaDadaDeBaja);
        await RefrescarAsync();
    });

    /// <summary>Vuelve a consultar con los filtros vigentes y deja la ficha vacía (ACTUALIZAR USR-PNL-CUENTAS Y USR-TBL-USUARIOS).</summary>
    private async Task RefrescarAsync()
    {
        var nombre = Busqueda;
        var rol = FiltroRolSeleccionado == TodosLosRoles ? (Rol?)null : Enum.Parse<Rol>(FiltroRolSeleccionado);

        try
        {
            MostrarCuentas(await Task.Run(() => _servicio.Consultar(nombre, rol)));
        }
        catch (Exception ex)
        {
            // La operación ya se completó: si solo falla el refresco, se informa sin deshacer nada.
            Registrar("Consulta posterior", ex);
            Mensaje = MensajesUsuarios.ErrorConsulta;
        }
    }

    private void MostrarCuentas(IReadOnlyList<CuentaUsuario> cuentas)
    {
        Cuentas.Clear();
        foreach (var cuenta in cuentas)
        {
            Cuentas.Add(cuenta);
        }

        CuentaSeleccionada = null;
        CargarFicha(null);
        Mensaje = Cuentas.Count == 0
            ? "No se encontraron cuentas."
            : $"Cuentas encontradas: {Cuentas.Count}.";
    }

    private void CargarFicha(CuentaUsuario? cuenta)
    {
        NombreUsuario = cuenta?.NombreUsuario ?? string.Empty;
        Contrasena = string.Empty;
        RolFicha = cuenta?.Rol;
        EstadoFicha = cuenta?.Estado ?? EstadoRegistro.Activo;
    }

    /// <summary>
    /// Ejecuta una operación con el tratamiento de errores común (DEC-072). Los demás módulos copian este método.
    /// </summary>
    private async Task EjecutarAsync(string operacion, string mensajeError, Func<Task> accion)
    {
        try
        {
            await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            _mensajes.MostrarError(MensajesUsuarios.Titulo, ex.Message);
        }
        catch (AccesoDenegadoException ex)
        {
            Registrar(operacion, ex);
            _mensajes.MostrarError(MensajesUsuarios.Titulo, MensajesUsuarios.AccesoDenegado);
        }
        catch (Exception ex)
        {
            Registrar(operacion, ex);
            _mensajes.MostrarError(MensajesUsuarios.Titulo, mensajeError);
        }
    }

    private static void Registrar(string operacion, Exception excepcion)
    {
        Trace.TraceError("[Usuarios] {0}: {1}", operacion, excepcion);
    }
}
