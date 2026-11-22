using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.USR;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>
/// VistaModelo de la ventana principal: contiene la pantalla activa, arma el menú para el rol de la sesión
/// y gobierna el inicio y el cierre de sesión (VP-00).
/// </summary>
/// <remarks>
/// <para>Al arrancar no hay sesión: muestra el inicio de sesión y el menú está vacío. Cuando la sesión se abre, arma el
/// menú con el rol de la cuenta, con las opciones que el rol no permite deshabilitadas y no ocultas (RNF-0006,
/// condición 4; DEC-075), y pasa a la pantalla de inicio. Al cerrar sesión borra la sesión, vacía el menú y vuelve al
/// inicio de sesión (DEC-080).</para>
/// </remarks>
public class MainViewModel : ViewModelBase
{
    private static readonly IReadOnlyList<ModuloMenuViewModel> SinModulos = Array.Empty<ModuloMenuViewModel>();

    private readonly IAutenticacion _autenticacion;
    private readonly ISesion _sesion;
    private readonly IReadOnlyList<ModuloMenu> _modulosRegistrados;

    private IReadOnlyList<ModuloMenuViewModel> _modulos = SinModulos;
    private string _usuarioSesion = string.Empty;

    public MainViewModel(
        INavegacionServicio navegacion,
        IAutenticacion autenticacion,
        ISesion sesion,
        IEnumerable<ModuloMenu> modulosRegistrados)
    {
        Navegacion = navegacion ?? throw new ArgumentNullException(nameof(navegacion));
        _autenticacion = autenticacion ?? throw new ArgumentNullException(nameof(autenticacion));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
        ArgumentNullException.ThrowIfNull(modulosRegistrados);
        _modulosRegistrados = modulosRegistrados.ToArray();

        CerrarSesionCommand = new RelayCommand(CerrarSesion, () => HaySesion);

        MostrarInicioSesion();
    }

    public INavegacionServicio Navegacion { get; }

    /// <summary>Si hay una sesión iniciada: solo entonces se muestran el menú y el cierre de sesión.</summary>
    public bool HaySesion => _autenticacion.HaySesion;

    /// <summary>Los módulos registrados; los que el rol no puede abrir quedan deshabilitados (DEC-075). Vacío sin sesión.</summary>
    public IReadOnlyList<ModuloMenuViewModel> Modulos
    {
        get => _modulos;
        private set => Establecer(ref _modulos, value);
    }

    /// <summary>Texto «usuario · rol» de la sesión, para mostrar en la barra de menú; vacío sin sesión.</summary>
    public string UsuarioSesion
    {
        get => _usuarioSesion;
        private set => Establecer(ref _usuarioSesion, value);
    }

    /// <summary>USR-BTN-CERRAR-SESION.</summary>
    public RelayCommand CerrarSesionCommand { get; }

    public string Titulo => "FARMASIL";

    private void MostrarInicioSesion()
    {
        Navegacion.NavegarA<InicioSesionViewModel>();

        // La navegación crea la VistaModelo de la pantalla; la ventana escucha su aviso para armar el menú.
        if (Navegacion.Actual is InicioSesionViewModel inicio)
        {
            inicio.SesionIniciada += AlIniciarSesion;
        }
    }

    private void AlIniciarSesion(object? emisor, EventArgs argumentos)
    {
        if (emisor is InicioSesionViewModel inicio)
        {
            inicio.SesionIniciada -= AlIniciarSesion;
        }

        var usuario = _sesion.UsuarioActual;
        if (!Enum.IsDefined(usuario.Rol))
        {
            throw new InvalidOperationException($"La sesión tiene un rol desconocido: {usuario.Rol}.");
        }

        Modulos = _modulosRegistrados
            .Select(modulo => new ModuloMenuViewModel(modulo, usuario.Rol))
            .ToArray();
        UsuarioSesion = $"{usuario.NombreUsuario} · {usuario.Rol}";
        NotificarSesion();

        Navegacion.NavegarA<InicioViewModel>();
    }

    private void CerrarSesion()
    {
        _autenticacion.CerrarSesion();

        Modulos = SinModulos;
        UsuarioSesion = string.Empty;
        NotificarSesion();

        MostrarInicioSesion();
    }

    private void NotificarSesion()
    {
        Notificar(nameof(HaySesion));
        CerrarSesionCommand.NotificarPuedeEjecutar();
    }
}
