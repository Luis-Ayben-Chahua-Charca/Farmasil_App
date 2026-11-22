using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.USR;

namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Sesión real de la aplicación: reemplaza a <see cref="SesionSimulada"/> (#88). Implementa <see cref="ISesion"/>, que
/// ven los módulos, e <see cref="IAutenticacion"/>, que usan el inicio de sesión y la ventana principal.
/// </summary>
/// <remarks>
/// <para><b>Una sola sesión.</b> La aplicación la usa una persona a la vez (RNF-0003 v02.00): hay un único usuario
/// activo y un inicio de sesión lo reemplaza solo si las credenciales son válidas.</para>
/// <para><b>Qué se verifica (RNF-0006, condición 1).</b> La contraseña contra el hash con sal y, solo si es correcta,
/// el estado de la cuenta. Un usuario inexistente y una contraseña errónea dan el mismo mensaje, y el usuario
/// inexistente también paga el costo del hash, para que el tiempo de respuesta no revele qué usuarios existen.</para>
/// <para><b>Credenciales.</b> La contraseña y el hash no se guardan, no se escriben en ningún registro y no salen de
/// este método (condición 5). <see cref="UsuarioActual"/> es un <see cref="SesionUsuario"/>, sin hash.</para>
/// </remarks>
public sealed class SesionActual : ISesion, IAutenticacion
{
    private readonly IUsuariosRepositorio _repositorio;
    private readonly IHashContrasenas _hash;
    private readonly Lazy<string> _hashFicticio;

    // Se lee desde la interfaz y se escribe desde un hilo de fondo: volatile publica la referencia completa.
    private volatile SesionUsuario? _usuario;

    public SesionActual(IUsuariosRepositorio repositorio, IHashContrasenas hash)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
        _hashFicticio = new Lazy<string>(() => hash.Generar(Guid.NewGuid().ToString("N")));
    }

    public bool HaySesion => _usuario is not null;

    /// <exception cref="AccesoDenegadoException">No hay sesión iniciada.</exception>
    public SesionUsuario UsuarioActual => _usuario ?? throw new AccesoDenegadoException(MensajesSesion.SinSesion);

    public void IniciarSesion(string? nombreUsuario, string? contrasena)
    {
        // Datos vacíos no pueden coincidir con ninguna cuenta: reciben el mismo rechazo, sin consultar la base.
        var nombre = nombreUsuario?.Trim();
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contrasena))
        {
            throw new ReglaDeNegocioException(MensajesSesion.CredencialesIncorrectas);
        }

        var cuenta = _repositorio.ObtenerCredenciales(nombre);
        if (cuenta is null)
        {
            _ = _hash.Verificar(contrasena, _hashFicticio.Value);
            throw new ReglaDeNegocioException(MensajesSesion.CredencialesIncorrectas);
        }

        if (!_hash.Verificar(contrasena, cuenta.Hash))
        {
            throw new ReglaDeNegocioException(MensajesSesion.CredencialesIncorrectas);
        }

        if (cuenta.Estado != EstadoRegistro.Activo)
        {
            throw new ReglaDeNegocioException(MensajesSesion.CuentaInactiva);
        }

        _usuario = new SesionUsuario
        {
            IdUsuario = cuenta.IdUsuario,
            NombreUsuario = cuenta.NombreUsuario,
            Rol = cuenta.Rol,
        };
    }

    public void CerrarSesion() => _usuario = null;

    public void ExigirRol(Rol rol)
    {
        if (!Enum.IsDefined(rol))
        {
            throw new ArgumentOutOfRangeException(nameof(rol), rol, "Rol desconocido.");
        }

        var usuario = UsuarioActual;
        if (usuario.Rol != rol)
        {
            throw new AccesoDenegadoException(
                $"La operación exige el rol {rol} y la sesión tiene el rol {usuario.Rol}.");
        }
    }
}
