using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.USR;

/// <summary>
/// Reglas del módulo de Usuarios (ESP-0033 a ESP-0036). Es el módulo de referencia: los demás módulos copian esta forma.
/// </summary>
/// <remarks>
/// <para><b>Dónde viven las reglas.</b> Toda regla de la especificación está en este servicio, no en la VistaModelo ni en
/// el SQL. El servicio recibe por inyección el repositorio, la sesión y el hash, así que cada regla se prueba con
/// dobles en memoria.</para>
/// <para><b>Cómo se informa un fallo.</b>
/// <list type="bullet">
/// <item>Cada operación empieza con <see cref="ISesion.ExigirRol"/>: una sesión 'Tecnico' recibe
/// <see cref="AccesoDenegadoException"/> (RNF-0006, condición 3; DEC-069).</item>
/// <item>Una validación o una guarda fallida lanza <see cref="ReglaDeNegocioException"/> con el texto de
/// <see cref="MensajesUsuarios"/>, antes de escribir nada (DEC-072).</item>
/// <item>Cualquier otra excepción es un error técnico del repositorio.</item>
/// </list></para>
/// <para><b>Normalización (DEC-074).</b> El nombre de usuario se recorta y uno formado solo por espacios cuenta como vacío;
/// la contraseña no se recorta y solo está vacía si no tiene caracteres.</para>
/// </remarks>
public sealed class UsuariosServicio
{
    /// <summary>Longitud de <c>TBL_USUARIOS.nombre_usuario</c>, <c>NVARCHAR(50)</c> (DEC-074).</summary>
    public const int LongitudMaximaNombre = 50;

    private readonly IUsuariosRepositorio _repositorio;
    private readonly ISesion _sesion;
    private readonly IHashContrasenas _hash;

    public UsuariosServicio(IUsuariosRepositorio repositorio, ISesion sesion, IHashContrasenas hash)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    /// <summary>
    /// ESP-0034. Filtra por nombre de usuario exacto y por rol; un criterio vacío no filtra. El resultado no lleva
    /// la contraseña ni su hash, porque <see cref="CuentaUsuario"/> no los tiene.
    /// </summary>
    public IReadOnlyList<CuentaUsuario> Consultar(string? nombreUsuario, Rol? rol)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (rol is not null && !Enum.IsDefined(rol.Value))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.RolInvalido);
        }

        var nombre = string.IsNullOrWhiteSpace(nombreUsuario) ? null : nombreUsuario.Trim();
        return _repositorio.Consultar(nombre, rol);
    }

    /// <summary>ESP-0033. Valida en el orden de la especificación, guarda el hash con sal y devuelve el id de la cuenta.</summary>
    public int Crear(string? nombreUsuario, string? contrasena, Rol? rol, EstadoRegistro? estado)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var nombre = ValidarNombre(nombreUsuario, excluirIdUsuario: null);

        if (string.IsNullOrEmpty(contrasena))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.ContrasenaVacia);
        }

        var rolValido = ValidarRol(rol);
        var estadoValido = ValidarEstado(estado);

        // Solo el hash llega al repositorio: la contraseña nunca se persiste (RNF-0006, condición 5).
        return _repositorio.Crear(nombre, _hash.Generar(contrasena), rolValido, estadoValido);
    }

    /// <summary>
    /// ESP-0035. Una contraseña vacía conserva el hash vigente; una nueva guarda un hash nuevo. Rechaza el cambio de rol
    /// o de estado de la cuenta de la sesión (DEC-073) y el que deje el sistema sin administradores activos.
    /// </summary>
    public void Actualizar(int idUsuario, string? nombreUsuario, string? contrasena, Rol? rol, EstadoRegistro? estado)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var nombre = ValidarNombre(nombreUsuario, excluirIdUsuario: idUsuario);
        var rolValido = ValidarRol(rol);
        var estadoValido = ValidarEstado(estado);

        var cuenta = _repositorio.Obtener(idUsuario)
            ?? throw new InvalidOperationException($"La cuenta {idUsuario} no existe.");

        if (idUsuario == _sesion.UsuarioActual.IdUsuario
            && (rolValido != cuenta.Rol || estadoValido != cuenta.Estado))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.CuentaPropiaRolEstado);
        }

        // RNF-0006, condición 6: si la cuenta deja de ser administradora activa, debe quedar otra.
        var dejaDeSerAdministradoraActiva = rolValido != Rol.Administrador || estadoValido != EstadoRegistro.Activo;
        if (dejaDeSerAdministradoraActiva && _repositorio.ContarAdministradoresActivos(idUsuario) == 0)
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.UltimoAdministrador);
        }

        var hashNuevo = string.IsNullOrEmpty(contrasena) ? null : _hash.Generar(contrasena);
        _repositorio.Actualizar(idUsuario, nombre, rolValido, estadoValido, hashNuevo);
    }

    /// <summary>
    /// ESP-0036, antes del modal: rechaza la baja de la cuenta de la sesión. La VistaModelo la llama antes de pedir la
    /// confirmación, porque la especificación cancela la operación sin mostrar el modal.
    /// </summary>
    public void VerificarBaja(int idUsuario)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idUsuario == _sesion.UsuarioActual.IdUsuario)
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.CuentaPropiaBaja);
        }
    }

    /// <summary>
    /// ESP-0036, después de la confirmación. Vuelve a aplicar <see cref="VerificarBaja"/> y delega la baja condicional
    /// en el repositorio: la base decide entre borrar y desactivar (DEC-009, DEC-029).
    /// </summary>
    public ResultadoBaja DarDeBaja(int idUsuario)
    {
        VerificarBaja(idUsuario);
        return _repositorio.DarDeBaja(idUsuario);
    }

    private string ValidarNombre(string? nombreUsuario, int? excluirIdUsuario)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.NombreVacio);
        }

        var nombre = nombreUsuario.Trim();

        if (nombre.Length > LongitudMaximaNombre)
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.NombreDemasiadoLargo);
        }

        if (_repositorio.ExisteNombre(nombre, excluirIdUsuario))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.NombreRepetido);
        }

        return nombre;
    }

    private static Rol ValidarRol(Rol? rol)
    {
        if (rol is null || !Enum.IsDefined(rol.Value))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.RolInvalido);
        }

        return rol.Value;
    }

    private static EstadoRegistro ValidarEstado(EstadoRegistro? estado)
    {
        if (estado is null || !Enum.IsDefined(estado.Value))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.EstadoInvalido);
        }

        return estado.Value;
    }
}
